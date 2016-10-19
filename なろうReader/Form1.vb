Imports System.IO
Imports System.Net.Sockets
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Speech.Synthesis
Imports System.Globalization
Public Class Form1


    Dim myTitle = Me.Text
    Dim novelTitle As String = ""
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
    Dim bList As New ArrayList
    Dim fList As New ArrayList
    Dim curURL As String = ""
    Dim curTitle As String = ""
    Dim oldUrl As String = ""
    Dim oldTitle As String = ""
    Dim hIndex As Integer = 0
    Dim talkStopped As Boolean = True
    Dim talkStart As Boolean = False
    Dim readingText As String = "読み上げ中..."
    Dim stoppingText As String = "読み上げ停止中"
    Dim noReadingText As String = "読み上げる文章がありません"
    Dim echoDir As String = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\" + My.Application.Info.Title
    Dim multiLoad As Integer = 0
    Dim nowTalking As Boolean = False
    Dim indexArray As Integer()
    Dim synth As SpeechSynthesizer
    Public SAPIvoices As New List(Of String)
    Public SAPIVoice As String
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        NoTalk()
        synth = New SpeechSynthesizer()
        Dim vv = synth.GetInstalledVoices(New CultureInfo("ja-JP"))
        SAPIVoice = ""
        If vv.Count > 0 Then
            For Each v As InstalledVoice In vv
                SAPIvoices.Add(v.VoiceInfo.Name)
                If v.VoiceInfo.Name = My.Settings.SAPIVoice Then
                    SAPIVoice = v.VoiceInfo.Name
                End If
            Next
            If SAPIVoice.Length = 0 Then
                SAPIVoice = SAPIvoices(0)
            End If
            synth.SelectVoice(SAPIVoice)
        Else
            synth.Dispose()
            synth = Nothing
        End If
        selectHome(My.Settings.home)
        WriteReadingLabel(readingText)
        myTitle = Me.Text
        bouyomicheck()
        Me.ClientSize = My.Settings.MyClientSize
        selectHome(My.Settings.home)
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
        tThread = New Thread(New ThreadStart(AddressOf Talk))
        tThread.IsBackground = True
        tThread.Name = "TalkThread"
        tThread.Start()

    End Sub
    Private Sub Sethome(target As String)
        Select Case target
            Case "小説家になろう"
                homeUrl = "http://syosetu.com/"
                My.Settings.home = "小説家になろう"
            Case "カクヨム"
                homeUrl = "https://kakuyomu.jp"
                My.Settings.home = "カクヨム"
            Case "アルファポリス"
                homeUrl = "http://www.alphapolis.co.jp/"
                My.Settings.home = "アルファポリス"
        End Select

    End Sub
    Private Sub selectHome(target As String)
        Dim index As Integer = ComboBox1.FindString(target)
        If index < 0 Then index = 0
        ComboBox1.SelectedIndex = index
    End Sub
    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
        WebBrowser1.GoBack()
    End Sub
    Private Function RubyConvert(content As HtmlElement) As String
        Dim elems As HtmlElementCollection
        Dim rubyChanged As Boolean = False
        Dim honbun As String = ""
        If My.Settings.rubyNothing Then
            Return content.InnerText
        End If
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
                        If My.Settings.rubyConvert Or My.Settings.rubyDelete Then
                            kakko.OuterHtml = ""
                        End If
                    Next
                    Dim rtcol As HtmlElementCollection = elem.GetElementsByTagName("RT")
                    For Each yomi As HtmlElement In rtcol
                        'ルビが傍点か読み仮名か調べる
                        If Regex.IsMatch(yomi.InnerText, "[^, .、。．・]") Then
                            isBouten = False
                        End If
                        If My.Settings.rubyConvert Then
                            yomiText = yomiText + yomi.InnerText
                        ElseIf My.Settings.rubyDelete Then
                            yomi.InnerHtml = ""
                        End If

                    Next
                    If isBouten Then
                        For Each yomi As HtmlElement In rtcol
                            '傍点を削除
                            If My.Settings.rubyConvert Or My.Settings.rubyDelete Then
                                yomi.InnerHtml = ""
                            End If

                        Next
                    Else
                        'ルビベースを読み仮名で置き換え
                        If My.Settings.rubyConvert Then
                            elem.OuterHtml = yomiText
                        End If
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
    Public Sub loadPlainText()
        Dim content As HtmlElement = Nothing
        Dim wtitle As HtmlElement = Nothing
        Dim maegaki As HtmlElement = Nothing
        Dim atogaki As HtmlElement = Nothing
        Dim maeText As String = ""
        Dim atoText As String = ""
        Dim author As String = ""

        Dim pageTitle As String = ""
        Dim siori As Boolean = False
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
        If bList.Count > 0 Then
            EnableButton(Button_Back)
        Else
            DisableButton(Button_Back)
        End If
        If fList.Count > 0 Then
            EnableButton(Button_Forward)
        Else
            DisableButton(Button_Forward)
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
            If content Is Nothing Then
                Dim divs As HtmlElementCollection = Doc.GetElementsByTagName("div")
                For Each el As HtmlElement In divs
                    ' カクヨム　本文
                    If el.GetAttribute("className") = "widget-episodeBody js-episode-body" Then
                        content = el
                        ' アルファポリス　本文
                    ElseIf el.GetAttribute("Classname") = "novel_body section target_scroll_navigation" Then
                        content = el
                        siori = True
                    End If
                Next
            End If
        End If
        If content IsNot Nothing Then
            'WebBrowser1.Stop()
            'アルファポリス　タイトル
            Dim hs As HtmlElementCollection = Doc.GetElementsByTagName("H1")
            For Each el As HtmlElement In hs
                Dim eclass As String = el.GetAttribute("className")
                If eclass = "novel_title" Then
                    title = el.InnerText + karagyou
                End If
            Next
            Dim divs As HtmlElementCollection = Doc.GetElementsByTagName("DIV")
            For Each el As HtmlElement In divs
                Dim eclass As String = el.GetAttribute("className")

                If eclass = "novel_bn" Or eclass = "novel_link section" Then

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
                    ' アルファポリス　作者
                ElseIf eclass = "novel_author section" Then
                    author = "作者 " + el.InnerText
                    ' アルファポリス　サブタイトル
                ElseIf eclass = "chapter_title" Then
                    'subtitle = el.InnerText
                End If


            Next

            If nextStory.Length = 0 Then
                ' カクヨム　次の話リンク
                Dim nextEp As HtmlElement = Doc.GetElementById("contentMain-nextEpisode")
                If nextEp IsNot Nothing Then
                    Dim nextLink As HtmlElementCollection = nextEp.GetElementsByTagName("A")
                    nextStory = nextLink(0).GetAttribute("href")
                End If
            End If
            If title.Length = 0 Then
                ' カクヨム タイトル
                Dim titleEl As HtmlElement = Doc.GetElementById("contentMain-header-workTitle")
                If titleEl IsNot Nothing Then title = titleEl.InnerText
                ' カクヨム　作者
                Dim authorEl As HtmlElement = Doc.GetElementById("contentMain-header-author")
                If authorEl IsNot Nothing Then author = Doc.GetElementById("contentMain-header-author").InnerText
            End If
            Dim ps As HtmlElementCollection = Doc.GetElementsByTagName("P")
            For Each el As HtmlElement In ps
                Dim eclass As String = el.GetAttribute("className")
                ' なろう　サブタイトル
                If eclass = "novel_subtitle" Then
                    subtitle = el.InnerText + karagyou
                    ' なろう　章タイトル
                ElseIf eclass = "chapter_title" Then
                    chapter = el.InnerText + karagyou
                    ' なろう　シリーズタイトル
                ElseIf eclass = "series_title" Then
                    title = el.InnerText + " "
                ElseIf eclass = "novel_title" Then
                    title = title + el.InnerText + " "
                ElseIf el.GetAttribute("className") = "chapterTitle level1" Then
                    subtitle = el.InnerText
                    ' カクヨム　章タイトル
                ElseIf el.GetAttribute("className") = "widget-episodeTitle" Then
                    subtitle += vbCrLf + el.InnerText + vbCrLf
                End If
            Next
            If author <> "" Then
                title = title + ControlChars.NewLine + author + karagyou
            End If
            novelTitle = title
            If My.Settings.readMaegaki Then
                maeText = RubyConvert(maegaki) + karagyou
            End If
            If My.Settings.readAtogaki Then
                atoText = karagyou + RubyConvert(atogaki)
            End If
            honbun = RubyConvert(content)

            If Not My.Settings.readTitle Then
                title = ""
            End If
            If Not My.Settings.readSubTitle Then
                subtitle = ""
            End If
            honbun = title + subtitle + maeText + honbun + atoText
            ' アルファポリス本文中の「しおりを挟む」を削除
            If siori Then
                honbun = Replace(honbun, vbCrLf + "しおりを挟む ", vbCrLf, 1, 2)
                honbun = Replace(honbun, vbCrLf + "しおり ", vbCrLf, 1, 2)
                siori = False
            End If
        Else
            honbun = ""
            NoTalk()
        End If
        'Catch
        '    honbun = ""
        'End Try

        TextBox1.Text = honbun
        If honbun.Length > 0 Then
            Dim sep As String
            sep = "\r\n"
            If My.Settings.separator = "改行と句点（。）" Then
                sep += "|。"
            ElseIf My.Settings.separator = "改行と句読点（。、）" Then
                sep += "|。|、"
            End If
            Dim r As New Regex(sep)
            Dim mc As MatchCollection = r.Matches(honbun)
            Dim i As Integer
            indexArray = New Integer(mc.Count) {}
            For i = 0 To mc.Count - 1
                indexArray(i) = mc.Item(i).Index
            Next
            If indexArray(i) < honbun.Length - 1 Then
                Array.Resize(indexArray, i + 2)
                indexArray(i + 1) = honbun.Length - 1
            End If
            TextBox1.SelectionStart = 0
            TextBox1.SelectionLength = 0
            TextBox1.Select(start, 0)
            TextBox1.ScrollToCaret()
            Debug.WriteLine("-----Scroll To top ----")
            EnableButton(playStopButton)
            SetButtonImage(playStopButton, 1)
            SetTip(playStopButton, "読み上げを停止します")
        End If
        If honbun.Length > 0 Then

            If My.Settings.autoRead Or (My.Settings.autoNext And reading) Then

                StartTalk()

            Else
                StopTalk()
            End If
        Else
            NoTalk()
        End If
        EnableButton(Button_reload)
        ProgressBar1.Hide()
    End Sub
    Private Sub WebBrowser1_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser1.DocumentCompleted


        If oldUrl = WebBrowser1.Url.ToString Then

            ProgressBar1.Hide()
            EnableButton(Button_reload)
            Thread.Yield()
            If Not WebBrowser1.ReadyState = WebBrowserReadyState.Complete Then
                Return
            End If
            Return

        Else
            TextBox1.Text = ""
            Thread.Yield()
        End If
        If Not WebBrowser1.ReadyState = WebBrowserReadyState.Complete Then
            EnableButton(Button_reload)
            ProgressBar1.Hide()
            Return
        End If
        multiLoad = multiLoad + 1
        Debug.WriteLine("multiLoad=" + multiLoad.ToString)

        If multiLoad > 1 Then
            Return
        End If
        If oldUrl <> WebBrowser1.Url.ToString Then
            'StopTalk()
        End If
        'StopTalk()

        Thread.Yield()
        Debug.WriteLine("------Parce HTML-----")
        If oldUrl.Length > 0 Then
            If hIndex = 0 Then
                If bList.Count > 0 Then
                    Dim bItem As Array = bList.Item(bList.Count - 1)
                    If bItem(0) = oldUrl Then
                        bList.RemoveAt(bList.Count - 1)
                    End If

                    fList.Clear()
                End If
                bList.Add({oldUrl, oldTitle})
            End If
            hIndex = 0
        End If
        curURL = WebBrowser1.Url.ToString
        curTitle = WebBrowser1.DocumentTitle
        oldUrl = curURL
        oldTitle = curTitle
        loadPlainText()

    End Sub
    Public Sub jtalk(text As String, opt As String)
        Dim ps1 As New Diagnostics.ProcessStartInfo
        ps1.FileName = "open_jtalk.exe"
        ps1.Arguments = opt
        ps1.CreateNoWindow = True
        ps1.UseShellExecute = False
        ps1.RedirectStandardInput = True
        ps1.WorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath)
        Dim hProcess As Diagnostics.Process = Diagnostics.Process.Start(ps1)
        Dim sw As StreamWriter = hProcess.StandardInput
        text = text.Trim
        sw.Write(text)
        sw.Close()
        hProcess.WaitForExit()
        hProcess.Close()
        hProcess.Dispose()
    End Sub
    Private Sub bouyomicheck()
        If Not My.Settings.useBouyomi Then
            Return
        End If
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
    Public Function jOpt(voice As String, a As Double, fm As Double, jm As Double, jf As Double, r As Double, g As Integer) As String
        Dim result = My.Settings.jtalk_opt
        result = result + " -m voices/" + voice + ".htsvoice"
        result = result + " -a " + a.ToString
        result = result + " -fm " + fm.ToString
        result = result + " -jm " + jm.ToString
        result = result + " -jf " + jf.ToString
        result = result + " -r " + r.ToString
        result = result + " -g " + g.ToString

        result = result + " -p 240 -u 0.2 -b 0.1 -s 48000"
        Return result
    End Function

    Private Sub stopbouyomi()
        If Not My.Settings.useBouyomi Then
            Return
        End If
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
        If Not My.Settings.useBouyomi Then
            Return False
        End If
        Thread.Sleep(200)
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
        tThread.Abort()
    End Sub
    Private Sub playStopButton_Click(sender As Object, e As EventArgs) Handles playStopButton.Click
        If playStopButton.ImageIndex = 0 Then
            start = TextBox1.SelectionStart
            reading = True
            oldStart = start
            length = 0
            StartTalk()
        Else
            reading = False
            StopTalk()
            stopbouyomi()
        End If
    End Sub

    Private Sub WebBrowser1_NewWindow(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles WebBrowser1.NewWindow
        Dim wb As WebBrowser = sender
        Dim txt As String = wb.StatusText
        If txt <> "" Then
            WebBrowser1.Navigate(txt)
        End If

        e.Cancel = True
    End Sub

    Private Delegate Sub dlgSetTip(b As Button, ByVal text As String)
    Public Sub SetTip(b As Button, ByVal text As String)
        If Me.Label_reading.InvokeRequired Then
            Static Dim d As dlgSetTip = New dlgSetTip(AddressOf Me.SetTip)
            Me.Label_reading.Invoke(d, b, text)
            Return
        End If
        ToolTip1.SetToolTip(b, text)
    End Sub

    Private Delegate Sub dlgSetReadingColor(fc As Color, bc As Color)
    Public Sub SetReadingColor(fc As Color, bc As Color)
        If Me.Label_reading.InvokeRequired Then
            Static Dim d As dlgSetReadingColor = New dlgSetReadingColor(AddressOf Me.SetReadingColor)
            Me.Label_reading.Invoke(d, fc, bc)
            Return
        End If
        Me.Label_reading.ForeColor = fc
        Me.Label_reading.BackColor = bc
    End Sub
    Private Delegate Sub dlgWriteRadinglabel(ByVal text As String)
    Public Sub WriteReadingLabel(ByVal text As String)
        If Me.Label_reading.InvokeRequired Then
            Static Dim d As dlgWriteRadinglabel = New dlgWriteRadinglabel(AddressOf Me.WriteReadingLabel)
            Me.Label_reading.Invoke(d, text)
            Return
        End If
        Me.Label_reading.Text = text
    End Sub
    Private Delegate Sub dlgShowReading()
    Public Sub ShowReading()
        If Me.Label_reading.InvokeRequired Then
            Static Dim d As dlgShowReading = New dlgShowReading(AddressOf Me.ShowReading)
            Me.Label_reading.Invoke(d)
            Return
        End If
        Me.Label_reading.Show()
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
    Private Delegate Sub dlgsetButtonImage(b As Button, ByVal i As Integer)
    Public Sub SetButtonImage(b As Button, ByVal i As Integer)
        If b.InvokeRequired Then
            Static Dim d As dlgsetButtonImage = New dlgsetButtonImage(AddressOf Me.SetButtonImage)
            b.Invoke(d, b, i)
        End If
        b.ImageIndex = i
    End Sub
    Private Delegate Sub dlgsetButtonInabled(b As Button, ByVal i As Boolean)
    Public Sub SetButtonEnabld(b As Button, ByVal i As Boolean)
        If b.InvokeRequired Then
            Static Dim d As dlgsetButtonInabled = New dlgsetButtonInabled(AddressOf Me.SetButtonEnabld)
            b.Invoke(d, b, i)
        End If
        b.Enabled = i
    End Sub
    Private Delegate Function dlgGetReaderText() As String
    Public Function GetReaderText() As String
        If Me.TextBox1.InvokeRequired Then
            Static Dim d As dlgGetReaderText = New dlgGetReaderText(AddressOf Me.GetReaderText)
            Return TextBox1.Invoke(d)
        End If
        Return TextBox1.Text
    End Function
    Private Delegate Function dlgGetSelectedText() As String
    Public Function GetSelectedText() As String
        If Me.TextBox1.InvokeRequired Then
            Static Dim d As dlgGetSelectedText = New dlgGetSelectedText(AddressOf Me.GetSelectedText)
            Return TextBox1.Invoke(d)

        End If
        Return TextBox1.SelectedText
    End Function
    Private Delegate Function dlgGetReaderLength() As Integer
    Public Function GetReaderLength() As Integer
        If Me.TextBox1.InvokeRequired Then
            Static Dim d As dlgGetReaderLength = New dlgGetReaderLength(AddressOf Me.GetReaderLength)
            TextBox1.Invoke(d)
        End If
        Return TextBox1.Text.Length
    End Function
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


    Private Function findNextPosition(ByRef a As Array, start As Integer) As Integer
        Dim i As Integer = 0
        Debug.WriteLine("Start Position=" + start.ToString)
        While i < a.Length
            If start >= 0 And start <= a(0) Then
                Debug.WriteLine("Result=" + a(0).ToString)
                Return a(0)
            ElseIf i > 0 Then
                If start > a(i - 1) And start <= a(i) Then
                    Debug.WriteLine("Result=" + a(i).ToString)
                    Return a(i)
                End If
            End If
            i = i + 1
        End While
        Debug.WriteLine("Result=" + start.ToString)
        Return start
    End Function

    Private Sub Talk()
        ' テキスト読み上げ処理：Threadで実行される
        Dim lStart As Integer = start
        Dim llength As Integer = length
        Dim oldLength As Integer = length
        Dim text1 As String
        Dim text2 As String
        Dim echofile As String = echoDir + "\echo.txt"
        Dim r As New Regex("(、|。|\r\n)+")
        While True
            text1 = GetReaderText()
            Dim mc As MatchCollection = r.Matches(text1)
            Dim src As String = text1
            Dim textLength As Integer = src.Length
            If (textLength < 1) Then
                reading = False
                NoTalk()
            Else
                Dim lineend As Int32
                Try
                    lineend = src.IndexOf(ControlChars.NewLine, lStart)
                Catch
                    lineend = lStart
                End Try
                lineend = findNextPosition(indexArray, lStart)
                If lineend = -1 Then
                    lineend = textLength
                End If
                llength = lineend - lStart + 1
                If llength >= 0 Then
                    Try
                        src = src.Substring(lStart, llength)
                    Catch
                        src = src.Substring(lStart, 0)
                    End Try
                End If
                src = src.Trim
                DoSelect(lStart, llength)
                DoScroll()
                Debug.WriteLine(src)
                If src.Length > 0 Then
                    Thread.Yield()
                    Dim echoOut As StreamWriter
                    Try
                        echoOut = New StreamWriter(echofile, False, System.Text.Encoding.UTF8)
                        echoOut.Write(src)
                        echoOut.Close()
                    Catch ex As Exception
                        If echoOut IsNot Nothing Then
                            echoOut.Close()
                        End If
                    End Try
                    If My.Settings.useBouyomi Then
                        bouyomi(src)
                        Dim st As New Stopwatch
                        st.Start()
                        While Not isTalking()
                            If talkStopped Then Exit While
                            '実際に発音し始めるまで待つ。何らかのエラーで発音しない状態が続くとまずいので2秒経過したら抜ける
                            Dim et As TimeSpan = st.Elapsed
                            If et.TotalSeconds > 2 Then
                                Exit While
                            End If
                            'Thread.Sleep(100)
                        End While
                        st.Stop()
                        While isTalking()
                            If talkStopped Then Exit While
                            'Thread.Sleep(100)
                        End While
                    ElseIf My.Settings.useJTalk Then
                        Dim opt As String = jOpt(My.Settings.jtalk_voice, My.Settings.jtalk_a, My.Settings.jtalk_fm, My.Settings.jtalk_jm, My.Settings.jtalk_jf, My.Settings.jtalk_r, My.Settings.jTalk_g)
                        jtalk(src, opt)
                    Else
                        SAPITalk(src, My.Settings.SAPIVolume, My.Settings.SAPIRate)
                    End If
                End If

                oldStart = lStart
                oldLength = llength
                lStart = lStart + llength '+ 1

                If textLength > 0 And lStart >= textLength Then
                    DoSelect(textLength, 0)
                    Dim nexturl As String = nextStory
                    If nextStory.Length > 0 And My.Settings.autoNext Then
                        loadURL(nextStory)
                        start = 0
                        length = 0
                        lStart = start
                        llength = 0
                        Thread.Sleep(1000)
                    Else
                        lStart = textLength - 1
                        llength = 0
                        StopTalk()
                    End If
                End If
            End If
            If GetReaderLength() = 0 Then
                StopTalk()
                talkStopped = True
            End If
            While talkStopped
                Thread.Sleep(100)
                If talkStart Then
                    lStart = start
                    llength = 0
                    talkStart = False
                    talkStopped = False
                    Thread.Sleep(100)
                    Exit While
                End If

            End While
            text2 = GetReaderText()
            If Not text1 = text2 Then
                '不意のページ遷移対策
                lStart = start
                llength = 0
                Thread.Sleep(100)
                Continue While
            End If
        End While
    End Sub
    Public Sub SAPITalk(str As String, vol As Integer, rate As Integer)
        If synth IsNot Nothing Then
            synth.Volume = vol
            synth.Rate = rate
            synth.Speak(str)
        End If
    End Sub
    Public Sub bouyomi(str As String)
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
            Dim ns As NetworkStream = tc.GetStream()
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

    Private Sub StartTalk()
        Dim ecohTiteFile As String = echoDir + "\title.txt"
        Dim echoOut As StreamWriter
        Try
            echoOut = New StreamWriter(ecohTiteFile, False, System.Text.Encoding.UTF8)
            echoOut.Write("小説家になろう:" + novelTitle)
            echoOut.Close()
        Catch ex As Exception
            If echoOut IsNot Nothing Then
                echoOut.Close()
            End If

        End Try


        talkStopped = True
        talkStart = True
        nowTalking = True
        EnableButton(playStopButton)
        SetButtonImage(playStopButton, 1)
        SetTip(playStopButton, "読み上げを停止します")
        WriteReadingLabel(readingText)
        Thread.Sleep(100)
        BlinkTimer.Start()
    End Sub
    Private Sub StopTalk()
        Dim echoTiteFile As String = echoDir + "\title.txt"
        Dim echoOut As StreamWriter
        Try
            echoOut = New StreamWriter(echoTiteFile, False, System.Text.Encoding.UTF8)
            echoOut.Write("")
            echoOut.Close()
        Catch ex As Exception
            If echoOut IsNot Nothing Then
                echoOut.Close()
            End If
        End Try
        Dim echoFile As String = echoDir + "\echo.txt"
        Dim ww As StreamWriter
        Try
            ww = New StreamWriter(echoFile, False, System.Text.Encoding.UTF8)
            ww.Write("")
            ww.Close()
        Catch ex As Exception
            If ww IsNot Nothing Then
                ww.Close()
            End If
        End Try
        talkStopped = True
        nowTalking = True
        EnableButton(playStopButton)
        SetButtonImage(playStopButton, 0)
        WriteReadingLabel(stoppingText)
        SetTip(playStopButton, "読み上げを開始します")
        BlinkTimer.Stop()
        'SetReadingColor(Color.Black, Color.Transparent)
        ShowReading()
        Thread.Sleep(100)
    End Sub
    Private Sub NoTalk()
        DisableButton(playStopButton)
        SetButtonImage(playStopButton, 0)
        WriteReadingLabel(noReadingText)

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

    Private Sub WebBrowser1_Navigating(sender As Object, e As WebBrowserNavigatingEventArgs) Handles WebBrowser1.Navigating
        If e.Url.ToString.Contains("about:blank") Then
            'WebBrowser1.Stop()
            'WebBrowser1.AllowNavigation = False
            Timer1.Interval = 50000
            Timer1.Start()
            Return
        End If
    End Sub


    Private Sub WebBrowser1_Navigated(sender As Object, e As WebBrowserNavigatedEventArgs) Handles WebBrowser1.Navigated
        WebBrowser1.AllowNavigation = True
        TextBox_url.Text = WebBrowser1.Url.ToString
        ProgressBar1.Show()
        DisableButton(Button_reload)
        Button_reload.Enabled = True
        multiLoad = 0
        Debug.WriteLine("Navigated:" + e.Url.ToString)
    End Sub

    Private Sub Button_Back_Click(sender As Object, e As EventArgs) Handles Button_Back.Click
        Dim num As Integer = bList.Count - 1
        If num >= 0 Then
            Dim item As Array = bList.Item(num)
            Dim curItem As Array = {oldUrl, oldTitle}
            Dim url = item(0)
            fList.Add(curItem)
            hIndex = -1
            bList.RemoveAt(num)
            loadURL(url)
        End If
    End Sub

    Private Sub Button_Forward_Click(sender As Object, e As EventArgs) Handles Button_Forward.Click
        Dim num As Integer = fList.Count - 1
        If num >= 0 Then
            Dim item As Array = fList.Item(num)
            Dim curItem As Array = {oldUrl, oldTitle}
            Dim url = item(0)
            bList.Add(curItem)
            hIndex = 1
            fList.RemoveAt(num)
            loadURL(url)
        End If
    End Sub

    Private Sub Button_reload_Click(sender As Object, e As EventArgs) Handles Button_reload.Click
        WebBrowser1.Refresh()
    End Sub

    Private Sub Button_home_Click(sender As Object, e As EventArgs) Handles Button_home.Click
        WebBrowser1.Navigate(homeUrl)
    End Sub
    Private Sub doSetting()
        StopTalk()
        start = 0
        length = 0
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

    Private Sub Button1_Enter(sender As Object, e As EventArgs)
        WebBrowser1.Select()
    End Sub

    Private Sub Button2_Enter(sender As Object, e As EventArgs)
        WebBrowser1.Select()
    End Sub
    Private Sub playStopButton_Enter(sender As Object, e As EventArgs) Handles playStopButton.Enter
        WebBrowser1.Select()
    End Sub

    Private Sub EnableButton(b As Button)
        SetButtonImage(b, 0)
        SetButtonEnabld(b, True)
    End Sub
    Private Sub DisableButton(b As Button)
        SetButtonImage(b, 1)
        SetButtonEnabld(b, False)
    End Sub

    Private Sub Button_Back_MouseDown(sender As Object, e As MouseEventArgs) Handles Button_Back.MouseDown
        Select Case e.Button
            Case MouseButtons.Right
                showBackMenu()
            Case MouseButtons.Left
                Timer2.Interval = 500
                Timer2.Start()
        End Select
    End Sub
    Private Sub showBackMenu()
        bMenu.Items.Clear()
        Dim max = bList.Count - 1
        Dim start = 0
        If max > 20 Then
            start = max - 20
        End If

        For i = start To max
            Dim newItem As New ToolStripMenuItem
            Dim item As Array = bList.Item(max - i + start)
            newItem.Text = item(1)
            newItem.Tag = i.ToString
            bMenu.Items.Add(newItem)
        Next

        bMenu.Show(Cursor.Position)


    End Sub

    Private Sub bMenu_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles bMenu.ItemClicked
        Dim strip As ContextMenuStrip = sender
        Dim item As ToolStripItem
        Dim num As Integer
        Dim max As Integer
        Dim start As Integer = 0

        item = e.ClickedItem
        num = Integer.Parse(item.Tag)
        max = bList.Count - 1
        If max > 20 Then
            start = max - 20
        End If
        Dim pItem As Array = {}
        For i = start To num
            pItem = bList.Item(max - i + start)
            fList.Add(pItem)
            bList.Remove(pItem)
        Next
        fList.RemoveAt(fList.Count - 1)
        fList.Insert(0, {oldUrl, oldTitle})
        hIndex = -num
        loadURL(pItem(0))

    End Sub
    Private Sub Button_Forward_MouseDown(sender As Object, e As MouseEventArgs) Handles Button_Forward.MouseDown, Button_Forward.MouseDown
        Select Case e.Button
            Case MouseButtons.Right
                showForwardMenu()
            Case MouseButtons.Left
                Timer3.Interval = 500
                Timer3.Start()
        End Select
    End Sub
    Private Sub showForwardMenu()
        fMenu.Items.Clear()
        Dim max = fList.Count - 1
        Dim start = 0
        If max > 20 Then
            start = max - 20
        End If

        For i =start To max
            Dim newItem As New ToolStripMenuItem
            Dim item As Array = fList.Item(max - i + start)
            newItem.Text = item(1)
            newItem.Tag = i.ToString
            fMenu.Items.Add(newItem)
        Next

        fMenu.Show(Cursor.Position)

    End Sub

    Private Sub fMenu_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles fMenu.ItemClicked
        Dim strip As ContextMenuStrip = sender
        Dim item As ToolStripItem
        Dim num As Integer
        Dim max As Integer
        Dim start As Integer = 0
        item = e.ClickedItem
        num = Integer.Parse(item.Tag)
        max = fList.Count - 1
        If max > 20 Then
            start = max - 20
        End If
        Dim pItem As Array = {}
        For i = start To num
            pItem = fList.Item(max - i + start)
            bList.Add(pItem)
            fList.Remove(pItem)
        Next
        bList.RemoveAt(bList.Count - 1)
        bList.Insert(0, {oldUrl, oldTitle})
        hIndex = num
        loadURL(pItem(0))

    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        showBackMenu()
        Timer2.Stop()
    End Sub
    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick
        showForwardMenu()
        Timer3.Stop()
    End Sub

    Private Sub Button_Back_MouseUp(sender As Object, e As MouseEventArgs) Handles Button_Back.MouseUp
        Timer2.Stop()

    End Sub
    Private Sub Button_Forward_MouseUp(sender As Object, e As MouseEventArgs) Handles Button_Forward.MouseUp
        Timer3.Stop()

    End Sub

    Private Sub BlinkTimer_Tick(sender As Object, e As EventArgs) Handles BlinkTimer.Tick
        Static show As Boolean = True
        If show Then
            show = False
            Label_reading.Show()
        Else
            show = True
            Label_reading.Hide()
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        WebBrowser1.Stop()
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Sethome(ComboBox1.SelectedItem.ToString)
        loadURL(homeUrl)
    End Sub
End Class
