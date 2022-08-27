Imports System.IO
Imports System.Net.Sockets
Imports System.Threading
Imports System.Text
Public Class Cliente
    Private MISTREAM As Stream
    Dim CLIENTE As TcpClient
    Dim THREADSERVIDOR As Thread

    Private Sub ButtonCONECTAR_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonCONECTAR.Click
        Try
            CheckForIllegalCrossThreadCalls = False
            CLIENTE = New TcpClient
            CLIENTE.Connect(TextBoxIP.Text, TextBoxPUERTO.Text)
            MISTREAM = CLIENTE.GetStream
            THREADSERVIDOR = New Thread(AddressOf LEER)
            THREADSERVIDOR.Start()
            ButtonCONECTAR.Enabled = False
            ButtonCONECTAR.Visible = False
            Me.Width = "780"
            Panel1.Visible = False
            Me.CenterToScreen()
            My.Settings.IP = TextBoxIP.Text
            My.Settings.Puerto = TextBoxPUERTO.Text
            My.Settings.Save()
            My.Settings.Reload()
            RichTextBox1.AppendText("Conectado al Servidor: " & TextBoxIP.Text & ":" & TextBoxPUERTO.Text)
        Catch ex As Exception
            Console.WriteLine("[Cliente@ButtonCONECTAR_Click]Error: " & ex.Message)
            MsgBox("A Ocurrido un Problema! " & vbCrLf & ex.Message, MsgBoxStyle.Information, "Worcome Security")
        End Try
        Label6.Text = "Server Info" & vbCrLf & "IP: " & TextBoxIP.Text & vbCrLf & "Port: " & TextBoxPUERTO.Text
    End Sub

    Private Sub LEER()
        Dim MIBUFFER() As Byte
        While True
            Try
                MIBUFFER = New Byte(100) {}
                MISTREAM.Read(MIBUFFER, 0, MIBUFFER.Length)
                Dim MENSAJE As String = Encoding.UTF7.GetString(MIBUFFER)
                If MENSAJE.Contains("AAAAAAAAAAAAAAAAA") Or MENSAJE.Contains("-") Then 'PARA EVITAR UN EXTRAÑO ¿ECO?
                    'ES ESE ¿ECO?
                Else
                    RichTextBox1.AppendText(vbCrLf & MENSAJE)
                    RichTextBox1.ScrollToCaret()
                    If MENSAJE.StartsWith("Conexion con:") Then
                        Dim MISPLIT As New ArrayList(MENSAJE.Split(":"))
                        ListBox1.Items.Add(MISPLIT(1))
                    ElseIf MENSAJE.StartsWith("Desconectado:") Then
                        Dim MISPLIT As New ArrayList(MENSAJE.Split(":"))
                        ListBox1.Items.Remove(MISPLIT(1))
                    End If
                    NotifyIcon1.ShowBalloonTip(3, "Wor MyChat", MENSAJE, ToolTipIcon.Info)
                End If


                'Codigo para Accionar los Comandos enviados por la CMD del Servidor
                If MENSAJE.Contains("---Servidor Cerrado---") Then
                    MsgBox("El Servidor fue Cerrado", MsgBoxStyle.Critical, "Worcome Security")
                    End
                End If

                If MENSAJE.Contains("CMD.Kick={ALL}") Then
                    MENSAJE.Replace("CMD.Kick={ALL}", Nothing)
                    RichTextBox1.Clear()
                    MsgBox("Todos los Usuarios fueron Expulsados del Servidor", MsgBoxStyle.Critical, "Worcome Security")
                    End
                End If

                If MENSAJE.Contains("CMD.Shutdown={ALL};-h") Then
                    MENSAJE.Replace("CMD.Shutdown={ALL};-h", Nothing)
                    MsgBox("Tu Computadora sera Apagada")
                    Process.Start("shutdown.exe", "/h")
                    End
                End If

                If MENSAJE.Contains("CMD.Aplication.Start=TaskMGR;{ALL}") Then
                    MENSAJE.Replace("CMD.Aplication.Start=TaskMGR;{ALL}", Nothing)
                    Process.Start("taskmgr.exe")
                End If

                If MENSAJE.Contains("CMD.Application.Start=Cmd") Then
                    MENSAJE.Replace("CMD.Application.Start=Cmd", Nothing)
                    Process.Start("cmd.exe")
                End If

            Catch ex As Exception
                Console.WriteLine("[Cliente@LEER]Error: " & ex.Message)
                Exit While
            End Try
        End While
    End Sub

    Public Sub ENVIAR(ByVal MENSAJE As String)
        Try
            Dim MIBUFFER() As Byte
            MIBUFFER = Encoding.UTF7.GetBytes(MENSAJE)
            MISTREAM.Write(MIBUFFER, 0, MIBUFFER.Length)
            TextBoxMENSAJE.Clear()
            TextBoxMENSAJE.Focus()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub Label1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label1.Click
        About.Show()
    End Sub

    Private Sub Cliente_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try
            ENVIAR(vbCrLf & TextBox1.Text & " se Desconecto")
            THREADSERVIDOR.Abort()
            CLIENTE.Close()
            End
        Catch ex As Exception
            Console.WriteLine("[Cliente@Cliente_FormClosing]Error: " & ex.Message)
            End
        End Try
    End Sub

    Private Sub Cliente_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        TextBox1.Text = My.Settings.UserNameCL
        TextBoxIP.Text = My.Settings.IP
        TextBoxPUERTO.Text = My.Settings.Puerto
        My.Settings.Save()
        My.Settings.Reload()
        If My.Settings.UserNameCL = Nothing Then
            TextBoxMENSAJE.ReadOnly = True
            TextBoxMENSAJE.Text = "Escriba un Nombre de Usuario"
            TextBox1.Focus()
        Else
            TextBox1.ReadOnly = True
            TextBoxMENSAJE.ReadOnly = False
            TextBoxMENSAJE.Clear()
        End If
    End Sub

    Private Sub TextBox1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            If TextBox1.Text = Nothing Then
                MsgBox("Debe escribir un Nombre de Usuario", MsgBoxStyle.Exclamation, "Worcome Security")
                TextBox1.Focus()
            Else
                ButtonCONECTAR.Enabled = True
                TextBox1.ReadOnly = True
                My.Settings.UserNameCL = TextBox1.Text
                TextBoxMENSAJE.ReadOnly = False
                TextBoxMENSAJE.Clear()
                TextBox1.ReadOnly = True
                My.Settings.Save()
                My.Settings.Reload()
            End If
        End If
    End Sub

    Private Sub TextBoxMENSAJE_KeyDown1(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBoxMENSAJE.KeyDown
        If e.KeyCode = Keys.Enter Then
            If TextBoxMENSAJE.Text = Nothing Then
                MsgBox("Escriba un Mensaje", MsgBoxStyle.Information, "Worcome Security")
                TextBoxMENSAJE.Focus()
            Else
                RichTextBox1.SelectionColor = Color.Blue
                ENVIAR(vbCrLf & "[User]" & TextBox1.Text & "> " & TextBoxMENSAJE.Text)
                RichTextBox1.ScrollToCaret()
                TextBoxMENSAJE.Clear()
                RichTextBox1.SelectionColor = Color.Black
            End If
        End If
    End Sub
End Class