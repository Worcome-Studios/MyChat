Public Class Inicio

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Servidor.Show()
        Me.Hide()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Cliente.Show()
        Me.Hide()
    End Sub

    Private Sub Label3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label3.Click
        About.Show()
    End Sub

    Sub AYUDA(ByVal globo As ToolTip, ByVal boton As Object, ByVal Mensaje As String)
        globo.RemoveAll()
        globo.SetToolTip(boton, Mensaje)
        globo.InitialDelay = 1000
        globo.IsBalloon = False
    End Sub

    Private Sub Inicio_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            If My.Settings.OfflineMode = False Then
                AppService.StartAppService(False, False, True, False, True)
                Threading.Thread.Sleep(150)
            End If
        Catch ex As Exception
            MsgBox("ERROR CRITICO CON 'AppService'", MsgBoxStyle.Critical, "Worcome Security")
        End Try
    End Sub

    Private Sub Button1_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.MouseEnter
        AYUDA(ToolTip1, Button1, "Crea un Servidor para que los Clientes se Conecten")
    End Sub

    Private Sub Button2_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button2.MouseEnter
        AYUDA(ToolTip1, Button2, "Inicia como Cliente para Conectarse al Servidor")
    End Sub
End Class