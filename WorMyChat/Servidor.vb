Imports System.Net.Sockets
Imports System.Threading
Imports System.Net
Imports System.Text
Public Class Servidor

    Dim SERVIDOR As TcpListener
    Dim CLIENTES As New Hashtable
    Dim THREADSERVIDOR As Thread
    Dim CLIENTEIP As IPEndPoint

    Public Structure NUEVOCLIENTE
        Public SOCKETCLIENTE As Socket
        Public THREADCLIENTE As Thread
        Public MENSAJE As String
    End Structure

    Public Sub Servidor_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        RichTextBox1.Clear()
        TextBox1.Text = My.Settings.UserNameSV
        TextBoxIP.Text = IPAddress.Any.ToString
        My.Settings.Save()
        My.Settings.Reload()
        If My.Settings.UserNameSV = "" Then
            TextBoxMENSAJE.ReadOnly = True
            TextBoxMENSAJE.Text = "Escriba un Nombre de Usuario"
            TextBox1.Focus()
        Else
            TextBox1.Enabled = False
            TextBoxMENSAJE.ReadOnly = False
            TextBoxMENSAJE.Clear()
            TextBoxMENSAJE.Focus()
            StartServer()
        End If
        CheckForIllegalCrossThreadCalls = False
    End Sub

    Sub StartServer()
        SERVIDOR = New TcpListener(IPAddress.Any, TextBoxPUERTO.Text)
        SERVIDOR.Start()
        THREADSERVIDOR = New Thread(AddressOf ESCUCHAR)
        THREADSERVIDOR.Start()
        RichTextBox1.AppendText("Servidor Abierto, Esperando Conexiones a tu Servidor...")
    End Sub

    Public Sub ESCUCHAR()
        Dim CLIENTE As New NUEVOCLIENTE
        While True
            Try
                CLIENTE.SOCKETCLIENTE = SERVIDOR.AcceptSocket
                CLIENTEIP = CLIENTE.SOCKETCLIENTE.RemoteEndPoint
                CLIENTE.THREADCLIENTE = New Thread(AddressOf LEER)
                CLIENTES.Add(CLIENTEIP, CLIENTE)
                NUEVACONEXION(CLIENTEIP)
                CLIENTE.THREADCLIENTE.Start()
            Catch ex As Exception
                Console.WriteLine("[Servidor@ESCUCHAR]Error: " & ex.Message)
            End Try
        End While
    End Sub

    Public Sub LEER()
        Dim CLIENTE As New NUEVOCLIENTE
        Dim DATOS() As Byte
        Dim IP As IPEndPoint = CLIENTEIP
        CLIENTE = CLIENTES(IP)
        While True
            If CLIENTE.SOCKETCLIENTE.Connected Then
                DATOS = New Byte(100) {}
                Try
                    If CLIENTE.SOCKETCLIENTE.Receive(DATOS, DATOS.Length, 0) > 0 Then
                        CLIENTE.MENSAJE = Encoding.UTF7.GetString(DATOS)
                        CLIENTES(IP) = CLIENTE
                        DatosRecibidos(IP)
                    Else
                        Exit While
                    End If
                Catch ex As Exception
                    Console.WriteLine("[Servidor@LEER]Error: " & ex.Message)
                    Exit While
                End Try
            End If
        End While
        Call CERRARTHREAD(IP)
    End Sub

    Public Sub CERRARTHREAD(ByVal IP As IPEndPoint)
        Dim CLIENTE As NUEVOCLIENTE = CLIENTES(IP)
        Try
            CLIENTE.THREADCLIENTE.Abort()
        Catch ex As Exception
            CLIENTES.Remove(IP)
            Console.WriteLine("[Servidor@CERRARTHREAD]Error: " & ex.Message)
        End Try
    End Sub

    Public Sub NUEVACONEXION(ByVal IDTerminal As IPEndPoint)
        RichTextBox1.AppendText(vbCrLf & "Conexion con:  " & IDTerminal.Address.ToString & ":" & IDTerminal.Port)
        RichTextBox1.ScrollToCaret()
        ListBox1.Items.Add(IDTerminal.Address.ToString & ":" & IDTerminal.Port)
        ENVIARTODOS("Conexion con:  " & IDTerminal.Address.ToString & ":" & IDTerminal.Port)
    End Sub

    Public Sub CONEXIONTERMINADA(ByVal IDTerminal As IPEndPoint)
        Try
            RichTextBox1.AppendText(vbCrLf & "Desconectado:  " & IDTerminal.Address.ToString & ":" & IDTerminal.Port)
            RichTextBox1.ScrollToCaret()
            ListBox1.Items.Remove(IDTerminal.Address.ToString & ":" & IDTerminal.Port)
            ENVIARTODOS("Desconectado:  " & IDTerminal.Address.ToString & ":" & IDTerminal.Port)
        Catch ex As Exception
            Console.WriteLine("[Servidor@CONEXIONTERMINADA]Error: " & ex.Message)
        End Try
    End Sub

    Public Sub DATOSRECIBIDOS(ByVal IDTerminal As IPEndPoint)
        RichTextBox1.AppendText(OBTENERDATOS(IDTerminal))
        RichTextBox1.ScrollToCaret()
        ENVIARTODOS(OBTENERDATOS(IDTerminal))
    End Sub

    Public Function OBTENERDATOS(ByVal IDCliente As IPEndPoint) As String
        Dim CLIENTE As NUEVOCLIENTE
        CLIENTE = CLIENTES(IDCliente)
        Return CLIENTE.MENSAJE
    End Function

    Public Sub CERRARUNO(ByVal IDCliente As IPEndPoint)
        Dim CLIENTE As NUEVOCLIENTE
        CLIENTE = CLIENTES(IDCliente)
        CLIENTE.SOCKETCLIENTE.Close()
        CLIENTE.THREADCLIENTE.Abort()
    End Sub

    Public Sub CERRARTODO()
        Dim CLIENTE As NUEVOCLIENTE
        For Each CLIENTE In CLIENTES.Values
            CLIENTE.SOCKETCLIENTE.Close()
            CLIENTE.THREADCLIENTE.Abort()
        Next
    End Sub

    Public Sub ENVIARUNO(ByVal IDCliente As IPEndPoint, ByVal Datos As String) ' A UN CLIENTE
        Dim Cliente As NUEVOCLIENTE
        Cliente = CLIENTES(IDCliente)
        Cliente.SOCKETCLIENTE.Send(Encoding.UTF7.GetBytes(Datos))
    End Sub

    Public Sub ENVIARTODOS(ByVal Datos As String) 'A TODOS LOS CLIENTES
        Dim CLIENTE As NUEVOCLIENTE
        For Each CLIENTE In CLIENTES.Values
            CLIENTE.SOCKETCLIENTE.Send(Encoding.UTF7.GetBytes(Datos))
        Next
    End Sub

    Private Sub ListBox1_DoubleClickedItem(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox1.MouseDoubleClick
        Try
            'ENVIARUNO(AlgoDebeIrAqui, InputBox("Escriba su Mensaje", "Worcome Security"))
        Catch ex As Exception
            Console.WriteLine("[Servidor@ListBox1_DoubleClickedItem]Error: " & ex.Message)
        End Try
    End Sub

    Public Sub TextBoxMENSAJE_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBoxMENSAJE.KeyDown
        If e.KeyCode = Keys.Enter Then
            If TextBoxMENSAJE.Text = Nothing Then
                MsgBox("Escriba un Mensaje", MsgBoxStyle.Information, "Worcome Security")
            Else
                ENVIARTODOS(vbCrLf & "[ADM]" & TextBox1.Text & "> " & TextBoxMENSAJE.Text)
                RichTextBox1.SelectionColor = Color.Blue
                RichTextBox1.AppendText(vbCrLf & "Tú> " & TextBoxMENSAJE.Text)
                RichTextBox1.SelectionColor = Color.Black
                RichTextBox1.ScrollToCaret()
                TextBoxMENSAJE.Clear()
            End If
        End If
    End Sub

    Public Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try
            ENVIARTODOS(vbCrLf & "---Servidor Cerrado---")
            ENVIARTODOS(vbCrLf & "-El Servidor fue Cerrado por: " & "[ADM]" & TextBox1.Text)
            Threading.Thread.Sleep(50)
            CERRARTODO()
            SERVIDOR.Stop()
            THREADSERVIDOR.Abort()
        Catch ex As Exception
            Console.WriteLine("[Servidor@Form1_FormClosing]Error: " & ex.Message)
            End
        End Try
    End Sub

    Private Sub Label3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label3.Click
        About.Show()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try
            Button1.Enabled = False
            Button1.Visible = False
            Me.Width = "780"
            Panel1.Visible = False
            Me.CenterToScreen()
            StartServer()
        Catch ex As Exception
            Console.WriteLine("[Servidor@Button1_Click]Error: " & ex.Message)
        End Try
        Label2.Text = "Server Info" & vbCrLf & "IP: " & TextBoxIP.Text & vbCrLf & "Port: " & TextBoxPUERTO.Text
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Command.Show()
    End Sub

    Sub AYUDA(ByVal globo As ToolTip, ByVal boton As Object, ByVal Mensaje As String)
        globo.RemoveAll()
        globo.SetToolTip(boton, Mensaje)
        globo.InitialDelay = 1000
        globo.IsBalloon = False
    End Sub

    Private Sub Button2_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button2.MouseEnter
        AYUDA(ToolTip1, Button2, "Ejecuta Comandos para el Servidor")
    End Sub

    Private Sub TextBoxPUERTO_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBoxPUERTO.TextChanged
        AYUDA(ToolTip1, TextBoxPUERTO, "Puerto de Recepcion y Envio (Default: 8080)")
    End Sub

    Private Sub TextBox1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            If TextBox1.Text = Nothing Then
                MsgBox("Debe escribir un Nombre de Usuario", MsgBoxStyle.Exclamation, "Worcome Security")
                TextBox1.Focus()
            Else
                TextBox1.ReadOnly = True
                My.Settings.UserNameSV = TextBox1.Text
                TextBoxMENSAJE.ReadOnly = False
                TextBoxMENSAJE.Clear()
                My.Settings.Save()
                My.Settings.Reload()
                'StartServer()
            End If
        End If
    End Sub
End Class