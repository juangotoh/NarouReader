Public Class Form2
    Dim bouyomiPath As String
    Dim myFont As Font
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        showFont(Form1.TextBox1.Font)
        CheckBox_autoRead.Checked = My.Settings.autoRead
        CheckBox_autoNext.Checked = My.Settings.autoNext
        CheckBox_Maegaki.Checked = My.Settings.readMaegaki
        CheckBox_Atogaki.Checked = My.Settings.readAtogaki
        CheckBox_readTitle.Checked = My.Settings.readTitle
        CheckBox_readSubTitle.Checked = My.Settings.readSubTitle
        RadioButton_rubyConvert.Checked = My.Settings.rubyConvert
        RadioButton_rubyDelete.Checked = My.Settings.rubyDelete
        RadioButton_rubyNothing.Checked = My.Settings.rubyNothing
        TrackBar_a.Value = My.Settings.jtalk_a * 100
        TrackBar_fm.Value = My.Settings.jtalk_fm * 10
        TrackBar_jm.Value = My.Settings.jtalk_jm * 100
        TrackBar_jf.Value = My.Settings.jtalk_jf * 100
        TrackBar_r.Value = My.Settings.jtalk_r * 10
        TrackBar_g.Value = My.Settings.jTalk_g
        TrackBar_SAPI_Speed.Value = My.Settings.SAPIRate
        TrackBar_SAPI_Volume.Value = My.Settings.SAPIVolume
        TrackToText(TrackBar_a, TextBox_a, 100)
        TrackToText(TrackBar_fm, TextBox_fm, 10)
        TrackToText(TrackBar_jm, TextBox_jm, 100)
        TrackToText(TrackBar_jf, TextBox_jf, 100)
        TrackToText(TrackBar_r, TextBox_r, 10)
        TrackToText(TrackBar_g, TextBox_g, 1)
        TrackToText(TrackBar_SAPI_Speed, TextBox_SAPI_Speed, 1)
        TrackToText(TrackBar_SAPI_Volume, TextBox_SAPI_Volume, 1)

        TextBox_bPath.Text = My.Settings.bouyomiPath
        ComboBox1.Text = My.Settings.jtalk_voice
        For Each v As String In Form1.SAPIvoices
            ComboBox_SAPI_Voice.Items.Add(v)
        Next
        Dim index As Integer
        index = ComboBox_SAPI_Voice.FindString(My.Settings.SAPIVoice)
        If index < 0 Then index = 0
        ComboBox_SAPI_Voice.SelectedIndex = index
        index = ComboBox_Separator.FindString(My.Settings.separator)
        If index < 0 Then index = 0
        ComboBox_Separator.SelectedIndex = index
        If My.Settings.useBouyomi Then
            RadioButton_bouyomi.Checked = True
            RadioButton_jtalk.Checked = False
            RadioButton_SAPI.Checked = False
            EnableBouyomi()
        ElseIf My.Settings.useJTalk Then
            RadioButton_bouyomi.Checked = False
            RadioButton_jtalk.Checked = True
            RadioButton_SAPI.Checked = False
            EnableJTalk()
        Else
            RadioButton_bouyomi.Checked = False
            RadioButton_jtalk.Checked = False
            RadioButton_SAPI.Checked = True
            EnableSAPI()
        End If
        myFont = Form1.TextBox1.Font
        Me.AcceptButton = OKButton
        Me.Button_Cancel = Button_Cancel
    End Sub
    Private Sub showFont(myFont)
        FontLabel.Text = myFont.Name + " , " + myFont.Size.ToString + " " + myFont.Style.ToString
        TextBox_sample.Font = myFont
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
        My.Settings.readMaegaki = CheckBox_Maegaki.Checked
        My.Settings.readAtogaki = CheckBox_Atogaki.Checked
        My.Settings.readTitle = CheckBox_readTitle.Checked
        My.Settings.readSubTitle = CheckBox_readSubTitle.Checked
        My.Settings.rubyConvert = RadioButton_rubyConvert.Checked
        My.Settings.rubyDelete = RadioButton_rubyDelete.Checked
        My.Settings.rubyNothing = RadioButton_rubyNothing.Checked
        My.Settings.useBouyomi = RadioButton_bouyomi.Checked
        My.Settings.useJTalk = RadioButton_jtalk.Checked
        My.Settings.useSAPI = RadioButton_SAPI.Checked
        My.Settings.jtalk_voice = ComboBox1.Text
        My.Settings.jtalk_a = Double.Parse(TextBox_a.Text)
        My.Settings.jtalk_fm = Double.Parse(TextBox_fm.Text)
        My.Settings.jtalk_jm = Double.Parse(TextBox_jm.Text)
        My.Settings.jtalk_jf = Double.Parse(TextBox_jf.Text)
        My.Settings.jtalk_r = Double.Parse(TextBox_r.Text)
        My.Settings.jTalk_g = Double.Parse(TextBox_g.Text)
        My.Settings.SAPIVoice = ComboBox_SAPI_Voice.Text
        Form1.SAPIVoice = My.Settings.SAPIVoice
        My.Settings.SAPIVolume = Integer.Parse(TextBox_SAPI_Volume.Text)
        My.Settings.SAPIRate = Integer.Parse(TextBox_SAPI_Speed.Text)
        My.Settings.separator = ComboBox_Separator.Text
        My.Settings.myFont = myFont
        Form1.TextBox1.Font = myFont
        My.Settings.bouyomiPath = TextBox_bPath.Text
        Form1.loadPlainText()
        Me.Close()
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        System.Diagnostics.Process.Start("http://chi.usamimi.info/Program/Application/BouyomiChan/")
    End Sub



    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged

    End Sub
    Private Sub TrackToText(tb As TrackBar, tt As TextBox, times As Double)
        Dim t As Double
        t = tb.Value / times
        tt.Text = t.ToString
    End Sub
    Private Sub TrackBar_a_Scroll(sender As Object, e As EventArgs) Handles TrackBar_a.Scroll
        TrackToText(sender, TextBox_a, 100)
    End Sub

    Private Sub TrackBar_fm_Scroll(sender As Object, e As EventArgs) Handles TrackBar_fm.Scroll
        TrackToText(sender, TextBox_fm, 10)
    End Sub

    Private Sub TrackBar_jm_Scroll(sender As Object, e As EventArgs) Handles TrackBar_jm.Scroll
        TrackToText(sender, TextBox_jm, 100)
    End Sub

    Private Sub TrackBar_jf_Scroll(sender As Object, e As EventArgs) Handles TrackBar_jf.Scroll
        TrackToText(sender, TextBox_jf, 100)
    End Sub

    Private Sub TrackBar_r_Scroll(sender As Object, e As EventArgs) Handles TrackBar_r.Scroll
        TrackToText(sender, TextBox_r, 10)
    End Sub

    Private Sub TrackBar_g_Scroll(sender As Object, e As EventArgs) Handles TrackBar_g.Scroll
        TrackToText(sender, TextBox_g, 1)
    End Sub
    Private Sub EnableBouyomi()
        ComboBox1.Enabled = False
        TextBox_a.Enabled = False
        TextBox_fm.Enabled = False
        TextBox_jm.Enabled = False
        TextBox_jf.Enabled = False
        TextBox_r.Enabled = False
        TextBox_g.Enabled = False
        TrackBar_a.Enabled = False
        TrackBar_fm.Enabled = False
        TrackBar_jm.Enabled = False
        TrackBar_jf.Enabled = False
        TrackBar_r.Enabled = False
        TrackBar_g.Enabled = False
        ComboBox_SAPI_Voice.Enabled = False
        TrackBar_SAPI_Speed.Enabled = False
        TrackBar_SAPI_Volume.Enabled = False
        TextBox_bPath.Enabled = True
        Button1.Enabled = True
    End Sub
    Private Sub EnableJTalk()
        ComboBox1.Enabled = True
        TextBox_a.Enabled = True
        TextBox_fm.Enabled = True
        TextBox_jm.Enabled = True
        TextBox_jf.Enabled = True
        TextBox_r.Enabled = True
        TextBox_g.Enabled = True
        TrackBar_a.Enabled = True
        TrackBar_fm.Enabled = True
        TrackBar_jm.Enabled = True
        TrackBar_jf.Enabled = True
        TrackBar_r.Enabled = True
        TrackBar_g.Enabled = True
        ComboBox_SAPI_Voice.Enabled = False
        TrackBar_SAPI_Speed.Enabled = False
        TrackBar_SAPI_Volume.Enabled = False
        TextBox_bPath.Enabled = False
        Button1.Enabled = False

    End Sub
    Private Sub EnableSAPI()
        ComboBox1.Enabled = False
        TextBox_a.Enabled = False
        TextBox_fm.Enabled = False
        TextBox_jm.Enabled = False
        TextBox_jf.Enabled = False
        TextBox_r.Enabled = False
        TextBox_g.Enabled = False
        TrackBar_a.Enabled = False
        TrackBar_fm.Enabled = False
        TrackBar_jm.Enabled = False
        TrackBar_jf.Enabled = False
        TrackBar_r.Enabled = False
        TrackBar_g.Enabled = False
        ComboBox_SAPI_Voice.Enabled = True
        TrackBar_SAPI_Speed.Enabled = True
        TrackBar_SAPI_Volume.Enabled = True
        TextBox_bPath.Enabled = False
        Button1.Enabled = False
    End Sub
    Private Sub RadioButton_bouyomi_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton_bouyomi.CheckedChanged
        EnableBouyomi()
    End Sub

    Private Sub RadioButton_jtalk_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton_jtalk.CheckedChanged
        EnableJTalk()
    End Sub

    Private Sub Button_Talk_Click(sender As Object, e As EventArgs) Handles Button_Talk.Click
        Dim str As String = "こんにちは、発音テストです"
        If RadioButton_bouyomi.Checked Then
            Form1.bouyomi(str)
        ElseIf RadioButton_jtalk.Checked Then
            Dim opt As String = Form1.jOpt(ComboBox1.Text, Double.Parse(TextBox_a.Text), Double.Parse(TextBox_fm.Text), Double.Parse(TextBox_jm.Text), Double.Parse(TextBox_jf.Text), Double.Parse(TextBox_r.Text), Double.Parse(TextBox_g.Text))
            Form1.jtalk(str, opt)
        Else
            Form1.SAPITalk(str, Integer.Parse(TextBox_SAPI_Volume.Text), Integer.Parse(TextBox_SAPI_Speed.Text))
        End If
    End Sub


    Private Sub TextBox_g_TextChanged(sender As Object, e As EventArgs) Handles TextBox_g.TextChanged

    End Sub

    Private Sub Label8_Click(sender As Object, e As EventArgs) Handles Label8.Click

    End Sub

    Private Sub TrackBar_SAPI_Speed_Scroll(sender As Object, e As EventArgs) Handles TrackBar_SAPI_Speed.Scroll
        TrackToText(sender, TextBox_SAPI_Speed, 1)
    End Sub

    Private Sub TrackBar_SAPI_Volume_Scroll(sender As Object, e As EventArgs) Handles TrackBar_SAPI_Volume.Scroll
        TrackToText(sender, TextBox_SAPI_Volume, 1)
    End Sub

    Private Sub RadioButton_SAPI_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton_SAPI.CheckedChanged
        EnableSAPI()
    End Sub
End Class