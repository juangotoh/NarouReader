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
        'FontDialogクラスのインスタンスを作成
        Dim fd As New FontDialog()

        '初期のフォントを設定
        fd.Font = Form1.TextBox1.Font

        '初期の色を設定
        fd.Color = Form1.TextBox1.ForeColor
        'ユーザーが選択できるポイントサイズの最大値を設定する
        fd.MaxSize = 64
        fd.MinSize = 9
        '存在しないフォントやスタイルをユーザーが選択すると
        'エラーメッセージを表示する
        fd.FontMustExist = True
        '横書きフォントだけを表示する
        fd.AllowVerticalFonts = False
        '色を選択できるようにする
        fd.ShowColor = False
        '取り消し線、下線、テキストの色などのオプションを指定可能にする
        'デフォルトがTrueのため必要はない
        fd.ShowEffects = True
        '固定ピッチフォント以外も表示する
        'デフォルトがFalseのため必要はない
        fd.FixedPitchOnly = False
        'ベクタ フォントを選択できるようにする
        'デフォルトがTrueのため必要はない
        fd.AllowVectorFonts = True

        'ダイアログを表示する
        If fd.ShowDialog() <> DialogResult.Cancel Then
            'TextBox1のフォントと色を変える
            myFont = fd.Font
            showFont(myFont)
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'OpenFileDialogクラスのインスタンスを作成
        Dim ofd As New OpenFileDialog()

        'はじめのファイル名を指定する
        'はじめに「ファイル名」で表示される文字列を指定する
        ofd.FileName = "BouyomiChan.exe"
        'はじめに表示されるフォルダを指定する
        '指定しない（空の文字列）の時は、現在のディレクトリが表示される
        ofd.InitialDirectory = "C:\"
        '[ファイルの種類]に表示される選択肢を指定する
        '指定しないとすべてのファイルが表示される
        ofd.Filter =
            "実行ファイル|*.exe"
        '[ファイルの種類]ではじめに
        '「すべてのファイル」が選択されているようにする
        ofd.FilterIndex = 2
        'タイトルを設定する
        ofd.Title = "Bouyomichan.exeを選択してください"
        'ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
        ofd.RestoreDirectory = True
        '存在しないファイルの名前が指定されたとき警告を表示する
        'デフォルトでTrueなので指定する必要はない
        ofd.CheckFileExists = True
        '存在しないパスが指定されたとき警告を表示する
        'デフォルトでTrueなので指定する必要はない
        ofd.CheckPathExists = True

        'ダイアログを表示する
        If ofd.ShowDialog() = DialogResult.OK Then
            'OKボタンがクリックされたとき
            '選択されたファイル名を表示する
            bouyomiPath = ofd.FileName
            TextBox_bPath.Text = bouyomiPath
            Console.WriteLine(ofd.FileName)
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
End Class