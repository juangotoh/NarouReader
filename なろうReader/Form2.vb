Public Class Form2
    Dim bouyomiPath As String
    Dim myFont As Font
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        showFont(Form1.TextBox1.Font)
        CheckBox_autoRead.Checked = My.Settings.autoRead
        CheckBox_autoNext.Checked = My.Settings.autoNext
        TextBox_bPath.Text = My.Settings.bouyomiPath
        myFont = Form1.TextBox1.Font
        Me.AcceptButton = OKButton
        Me.Button_Cancel = Button_Cancel
    End Sub
    Private Sub showFont(myFont)
        FontLabel.Text = myFont.Name + " , " + myFont.Size.ToString + " " + myFont.Style.ToString
    End Sub
    Private Sub Button_Font_Click(sender As Object, e As EventArgs) Handles Button_Font.Click
        FontDialog1.Font = Form1.TextBox1.Font

        If FontDialog1.ShowDialog() <> DialogResult.Cancel Then
            'TextBox1のフォントと色を変える
            myFont = FontDialog1.Font
            showFont(myFont)
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        OpenFileDialog1.FileName = "BouyomiChan.exe"
        OpenFileDialog1.InitialDirectory = "C:\"
        OpenFileDialog1.Filter = "実行ファイル|*.exe"
        OpenFileDialog1.FilterIndex = 0
        OpenFileDialog1.Title = "Bouyomichan.exeを選択してください"

        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            'OKボタンがクリックされたとき
            '選択されたファイル名を表示する
            bouyomiPath = OpenFileDialog1.FileName
            TextBox_bPath.Text = bouyomiPath
            Console.WriteLine(OpenFileDialog1.FileName)
        End If
    End Sub

    Private Sub CancelButton_Click(sender As Object, e As EventArgs) Handles Button_Cancel.Click
        Me.Close()
    End Sub

    Private Sub OKButton_Click(sender As Object, e As EventArgs) Handles OKButton.Click
        My.Settings.autoRead = CheckBox_autoRead.Checked
        My.Settings.autoNext = CheckBox_autoNext.Checked
        My.Settings.myFont = myFont
        Form1.TextBox1.Font = myFont
        My.Settings.bouyomiPath = TextBox_bPath.Text
        Me.Close()
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        System.Diagnostics.Process.Start("http://chi.usamimi.info/Program/Application/BouyomiChan/")
    End Sub
End Class