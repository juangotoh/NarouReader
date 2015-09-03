Imports System.IO
Imports System.Net.Sockets
Imports System.Text.RegularExpressions
Imports System.Threading
Public Class Form1
    Dim myTitle = Me.Text
    Dim reading As Boolean = False
    Dim firstRead As Boolean = True
    Dim title As String = ""
    Dim chapter As String = ""
    Dim subtitle As String = ""
    Dim honbun As String = ""
    Dim nextStory As String = ""
    Dim startpage As String = ""
    Dim karagyou As String = vbCrLf + vbCrLf
    Dim start As Int32
    Dim length As Int32
    Dim oldStart As Int32
    Dim bouyomiError As Boolean = False
    Dim homeUrl As String = "http://syosetu.com/"
    Dim myDialogOK As Boolean = False
    Dim tThread As Thread

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        NoTalk()
        myTitle = Me.Text
        bouyomicheck()
        Me.ClientSize = My.Settings.MyClientSize
        startpage = My.Settings.LastUrl
        start = My.Settings.LastSelection
        length = 0
        oldStart = 0
        WebBrowser1.ScriptErrorsSuppressed = True

        If startpage.Length = 0 Then startpage = homeUrl
        loadURL(startpage)
        TextBox1.HideSelection = False
        TextBox1.Font = My.Settings.myFont
        TextBox1.Text = ""


    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
        WebBrowser1.GoBack()
    End Sub
    Private Function RubyConvert(content As HtmlElement) As String
        Dim elems As HtmlElementCollection
        Dim rubyChanged As Boolean = False
        Dim honbun As String = ""
        Try
            Dim origHtml = content.InnerHtml

            elems = content.GetElementsByTagName("RUBY")
            If elems.Count > 0 Then
                For Each elem As HtmlElement In elems
                    Dim yomiText As String = ""
                    Dim isBouten As Boolean = True
                    Dim kakkoCol As HtmlElementCollection = elem.GetElementsByTagName("RP")
                    For Each kakko As HtmlElement In kakkoCol
                        '括弧は削除
                        kakko.OuterHtml = ""
                    Next
                    Dim rtcol As HtmlElementCollection = elem.GetElementsByTagName("RT")
                    For Each yomi As HtmlElement In rtcol
                        'ルビが傍点か読み仮名か調べる
                        If Regex.IsMatch(yomi.InnerText, "[^, .、。．・]") Then
                            isBouten = False
                        End If
                        yomiText = yomiText + yomi.InnerText
                    Next
                    If isBouten Then
                        For Each yomi As HtmlElement In rtcol
                            '傍点を削除
                            yomi.InnerHtml = ""
                        Next
                    Else
                        'ルビベースを読み仮名で置き換え
                        elem.OuterHtml = yomiText
                    End If


                Next
                honbun = content.InnerText

                'Webブラウザ画面でルビをいじった部分を元に戻す
                content.InnerHtml = origHtml
            Else
                honbun = content.InnerText
            End If
        Catch ex As Exception
            honbun = ""
        End Try
        Return honbun
    End Function
    Private Sub WebBrowser1_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser1.DocumentCompleted
        ProgressBar1.Hide()
        If e.Url <> DirectCast(sender, WebBrowser).Url Then
            Return
        End If

        EnableButton(Button_reload)
        Dim content As HtmlElement = Nothing
        Dim wtitle As HtmlElement = Nothing
        Dim maegaki As HtmlElement = Nothing
        Dim atogaki As HtmlElement = Nothing
        Dim maeText As String = ""
        Dim atoText As String = ""
        Dim author As String = ""

        Dim pageTitle As String = ""

        title = ""
        chapter = ""
        subtitle = ""
        nextStory = ""
        If Not (TextBox_url.Text = startpage) Then
            startpage = TextBox_url.Text
            start = 0
            oldStart = 0
            length = 0
        End If
        If WebBrowser1.CanGoBack Then
            Button_Back.ImageIndex = 0
            Button_Back.Enabled = True
        Else
            Button_Back.ImageIndex = 1
            Button_Back.Enabled = False
        End If
        If WebBrowser1.CanGoForward Then
            Button_Forward.ImageIndex = 0
            Button_Forward.Enabled = True
        Else
            Button_Forward.ImageIndex = 1
            Button_Forward.Enabled = False
        End If
        stopbouyomi()
        'Try
        Dim Doc As HtmlDocument = WebBrowser1.Document
        pageTitle = Doc.Title
        Me.Text = myTitle + " - " + pageTitle
        wtitle = Doc.GetElementById("writting_title")
        maegaki = Doc.GetElementById("novel_p")
        atogaki = Doc.GetElementById("novel_a")
        If wtitle IsNot Nothing Then
            title = wtitle.InnerText + karagyou
            Dim divs As HtmlElementCollection = Doc.GetElementsByTagName("div")
            For Each el As HtmlElement In divs
                If el.GetAttribute("className") = "writtingnovel novel" Then
                    content = el
                    Exit For
                End If
            Next

        Else
            content = Doc.GetElementById("novel_honbun")
        End If
        'content = Doc.GetElementById("novel_honbun")
        If content IsNot Nothing Then
            'origHtml = content.InnerHtml
            Dim divs As HtmlElementCollection = Doc.GetElementsByTagName("DIV")
            For Each el As HtmlElement In divs
                Dim eclass As String = el.GetAttribute("className")

                If eclass = "novel_bn" Then

                    Dim nextlink As HtmlElementCollection = el.GetElementsByTagName("A")
                    For Each l As HtmlElement In nextlink
                        Dim ltext As String = l.InnerText

                        If ltext.IndexOf("次の話") >= 0 Then
                            nextStory = l.GetAttribute("href")
                        End If
                    Next
                ElseIf eclass = "contents1" Then
                    title = el.InnerText + karagyou
                ElseIf eclass = "novel_writername" Then
                    author = el.InnerText
                End If


            Next

            Dim ps As HtmlElementCollection = Doc.GetElementsByTagName("P")
            For Each el As HtmlElement In ps
                Dim eclass As String = el.GetAttribute("className")
                If eclass = "novel_subtitle" Then
                    subtitle = el.InnerText + karagyou

                ElseIf eclass = "chapter_title" Then
                    chapter = el.InnerText + karagyou
                ElseIf eclass = "series_title" Then
                    title = el.InnerText + " "
                ElseIf eclass = "novel_title" Then
                    title = title + el.InnerText + " "
                End If
            Next
            If author <> "" Then
                title = title + ControlChars.NewLine + author + karagyou
            End If
            If My.Settings.readMaegaki Then
                maeText = RubyConvert(maegaki) + karagyou
            End If
            If My.Settings.readAtogaki Then
                atoText = RubyConvert(atogaki) + karagyou
            End If
            honbun = RubyConvert(content) + karagyou

            honbun = title + subtitle + maeText + honbun + atoText
        Else
            honbun = ""
            NoTalk()
        End If
        'Catch
        '    honbun = ""
        'End Try
        TextBox1.Text = honbun
        If honbun.Length > 0 Then
            TextBox1.SelectionStart = 0
            TextBox1.SelectionLength = 0
            TextBox1.Select(start, 0)
            TextBox1.ScrollToCaret()
            'start = 0
            'length = 0

            If My.Settings.autoRead Then
                StartTalk()
            Else
                StopTalk()
            End If
        Else
            NoTalk()
        End If
        ProgressBar1.Hide()
    End Sub
    Private Sub bouyomicheck()
        If Not System.IO.File.Exists(My.Settings.bouyomiPath) Then
            MessageBox.Show("棒読みちゃんの場所を設定してください", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
            doSetting()
        End If
        Dim ps As System.Diagnostics.Process() = System.Diagnostics.Process.GetProcesses()
        Dim bouyomiAlready As Boolean = False
        For Each p As System.Diagnostics.Process In ps
            Try

                If p.MainModule.FileName = My.Settings.bouyomiPath Then
                    bouyomiAlready = True
                    Exit For
                End If
            Catch ex As Exception

            End Try
        Next
        If My.Settings.bouyomiPath.Length > 0 And Not bouyomiAlready Then
            System.Diagnostics.Process.Start(My.Settings.bouyomiPath)
            Thread.Sleep(3000)
        End If


    End Sub
    Private Sub bouyomi(str As String)
        Dim bMessage As Byte()
        bMessage = System.Text.Encoding.UTF8.GetBytes(str)
        Dim length As Int32 = bMessage.Length
        Dim iCommand As Int16 = 1
        Dim iSpeed As Int16 = -1
        Dim iTone As Int16 = -1
        Dim iVolume As Int16 = -1
        Dim iVoice As Int16 = 0
        Dim bCode As Byte = 0
        Dim sHost As String = "127.0.0.1"
        Dim port As Integer = 50001
        Dim tc As TcpClient
        Try
            tc = New TcpClient(sHost, port)
            Dim ns = tc.GetStream()
            Dim bw As BinaryWriter = New BinaryWriter(ns)
            bw.Write(iCommand)
            bw.Write(iSpeed)
            bw.Write(iTone)
            bw.Write(iVolume)
            bw.Write(iVoice)
            bw.Write(bCode)
            bw.Write(length)
            bw.Write(bMessage)
            tc.Close()
        Catch ex As Exception
            If firstRead Then
                firstRead = False
                MessageBox.Show("棒読みちゃんが起動していないため、読み上げ機能が使えません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
                bouyomicheck()
                StopTalk()

            End If

        End Try
    End Sub
    Private Sub stopbouyomi()

        Dim iCommand As Int16 = 64

        Dim sHost As String = "127.0.0.1"
        Dim port As Integer = 50001
        Dim tc As TcpClient
        Try
            tc = New TcpClient(sHost, port)
            Dim ns = tc.GetStream()
            Dim bw As BinaryWriter = New BinaryWriter(ns)
            bw.Write(iCommand)
            tc.Close()
        Catch ex As Exception
            'MessageBox.Show("棒読みちゃんと通信できません", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub
    Private Function isTalking() As Boolean
        Dim iCommand As Int16 = 288
        Dim iResult As Byte = 0
        Dim sHost As String = "127.0.0.1"
        Dim port As Integer = 50001
        Dim tc As TcpClient
        Try
            tc = New TcpClient(sHost, port)
            Dim ns = tc.GetStream()
            Dim bw As BinaryWriter = New BinaryWriter(ns)
            Dim br As BinaryReader = New BinaryReader(bw.BaseStream)
            bw.Write(iCommand)

            iResult = br.ReadByte()
            tc.Close()
        Catch ex As Exception
            'MessageBox.Show("棒読みちゃんと通信できません", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        If iResult = 0 Then
            Return False
        Else
            Return True
        End If

    End Function
    Private Sub Form1_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        StopTalk()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        start = TextBox1.SelectionStart
        oldStart = start
        length = 0
        StartTalk()
    End Sub

    Private Sub WebBrowser1_NewWindow(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles WebBrowser1.NewWindow
        Dim wb As WebBrowser = sender
        Dim txt As String = wb.StatusText
        If txt <> "" Then
            WebBrowser1.Navigate(txt)
        End If

        e.Cancel = True
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        StopTalk()
        stopbouyomi()
    End Sub
    Private Delegate Sub dlgWriteText(ByVal text As String)
    Public Sub SetURL(ByVal text As String)
        If Me.TextBox_url.InvokeRequired Then ' Delete
            Static Dim d As dlgWriteText = New dlgWriteText(AddressOf Me.SetURL)
            Me.TextBox_url.Invoke(d, text)
            Return
        End If
        Me.TextBox_url.Text = text
    End Sub

    Delegate Sub DoSelectDelegate(ByVal start As Integer, ByVal len As Integer)
    Sub DoSelect(ByVal start As Integer, ByVal len As Integer)
        If InvokeRequired Then
            TextBox1.Invoke(New DoSelectDelegate(AddressOf TextBox1.Select), New Object() {start, len})
            Return
        End If
        TextBox1.Select(start, length)
    End Sub
    Delegate Sub DoScrollDelegate()
    Sub DoScroll()
        If InvokeRequired Then
            Invoke(New DoScrollDelegate(AddressOf TextBox1.ScrollToCaret))
            Return
        End If
        TextBox1.ScrollToCaret()
    End Sub

    Private Sub Talk()
        Dim lStart As Integer = start
        Dim llength As Integer = length
        While True
            'DoSelect(oldStart, start - oldStart + 1)
            'DoScroll()

            Dim src As String = TextBox1.Text
            If (TextBox1.Text.Length < 1) Then
                reading = False
                NoTalk()
                Return
            End If
            Dim lineend As Int32
            Try
                lineend = TextBox1.Text.IndexOf(ControlChars.NewLine, lStart)
            Catch
                lineend = lStart
            End Try
            If lineend = -1 Then
                lineend = TextBox1.Text.Length
            End If
            llength = lineend - lStart
            'TextBox1.SelectionStart = start
            If length >= 0 Then
                Try
                    src = src.Substring(lStart, llength)
                    Console.WriteLine(src + vbCrLf)
                Catch
                    src = src.Substring(lStart - 1, 0)
                End Try
            End If

            If src.Length > 0 Then
                DoSelect(lStart, llength)
                DoScroll()
                'Thread.Yield()
                bouyomi(src)
                Thread.Sleep(50)
                'TextBox1.Select(start, length)
                While isTalking()

                    'TextBox1.Select(oldStart, start - oldStart + 1)
                    'TextBox1.ScrollToCaret()
                    'Return
                    Thread.Sleep(50)
                End While

            End If

            'TextBox1.ScrollToCaret()
            oldStart = lStart
            lStart = lStart + llength + 1

            If lStart >= TextBox1.Text.Length Then

                Dim nexturl As String = nextStory
                If nextStory.Length > 0 And My.Settings.autoNext Then
                    loadURL(nextStory)
                Else
                    lStart = TextBox1.Text.Length
                End If
                Return
            End If
        End While
    End Sub

    Private Sub StartTalk()
        'Timer1.Start()

        If tThread IsNot Nothing Then
            tThread.Abort()
        End If
        tThread = New Thread(New ThreadStart(AddressOf Talk))
        tThread.IsBackground = True
        If Not tThread.IsAlive Then
            tThread.Start()
        End If

        EnableButton(Button2)
        DisableButton(Button1)
    End Sub
    Private Sub StopTalk()
        'Timer1.Stop()
        tThread.Abort()
        EnableButton(Button1)
        DisableButton(Button2)
    End Sub
    Private Sub NoTalk()
        DisableButton(Button1)
        DisableButton(Button2)
    End Sub
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        My.Settings.MyClientSize = Me.ClientSize
        My.Settings.LastUrl = TextBox_url.Text
        My.Settings.LastSelection = TextBox1.SelectionStart
        My.Settings.myFont = TextBox1.Font
    End Sub
    Private Sub loadURL(url As String)
        SetURL(url)
        WebBrowser1.Navigate(url)
    End Sub

    Private Sub TextBox_url_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox_url.KeyPress
        If e.KeyChar = Chr(13) Then
            e.Handled = True
            loadURL(TextBox_url.Text)
        End If
    End Sub

    Private Sub WebBrowser1_ProgressChanged(sender As Object, e As WebBrowserProgressChangedEventArgs) Handles WebBrowser1.ProgressChanged

    End Sub

    Private Sub WebBrowser1_Navigated(sender As Object, e As WebBrowserNavigatedEventArgs) Handles WebBrowser1.Navigated
        TextBox_url.Text = WebBrowser1.Url.ToString
        ProgressBar1.Show()
        DisableButton(Button_reload)
    End Sub

    Private Sub Button_Back_Click(sender As Object, e As EventArgs) Handles Button_Back.Click
        If WebBrowser1.CanGoBack Then
            WebBrowser1.GoBack()
        End If

    End Sub

    Private Sub Button_Forward_Click(sender As Object, e As EventArgs) Handles Button_Forward.Click
        If WebBrowser1.CanGoForward Then
            WebBrowser1.GoForward()
        End If
    End Sub

    Private Sub Button_reload_Click(sender As Object, e As EventArgs) Handles Button_reload.Click
        WebBrowser1.Refresh()
    End Sub

    Private Sub Button_home_Click(sender As Object, e As EventArgs) Handles Button_home.Click
        WebBrowser1.Navigate(homeUrl)
    End Sub
    Private Sub doSetting()
        Form2.StartPosition = FormStartPosition.CenterParent
        Form2.ShowDialog()
        If myDialogOK Then
            myDialogOK = False
            TextBox1.Font = My.Settings.myFont
        End If
        Form2.Dispose()
        bouyomicheck()
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button_Setting.Click
        doSetting()
    End Sub

    Private Sub Button_Setting_Enter(sender As Object, e As EventArgs) Handles Button_Setting.Enter
        WebBrowser1.Select()
    End Sub

    Private Sub Button_Back_Enter(sender As Object, e As EventArgs) Handles Button_Back.Enter
        WebBrowser1.Select()
    End Sub

    Private Sub Button_Forward_Enter(sender As Object, e As EventArgs) Handles Button_Forward.Enter
        WebBrowser1.Select()
    End Sub

    Private Sub Button_reload_Enter(sender As Object, e As EventArgs) Handles Button_reload.Enter
        WebBrowser1.Select()
    End Sub

    Private Sub Button_home_Enter(sender As Object, e As EventArgs) Handles Button_home.Enter
        WebBrowser1.Select()
    End Sub

    Private Sub Button1_Enter(sender As Object, e As EventArgs) Handles Button1.Enter
        WebBrowser1.Select()
    End Sub

    Private Sub Button2_Enter(sender As Object, e As EventArgs) Handles Button2.Enter
        WebBrowser1.Select()
    End Sub
    Private Sub EnableButton(b As Button)
        b.ImageIndex = 0
        b.Enabled = True
    End Sub
    Private Sub DisableButton(bouyomi As Button)
        bouyomi.ImageIndex = 1
        bouyomi.Enabled = False
    End Sub

End Class
