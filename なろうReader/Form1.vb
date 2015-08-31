Imports System.IO
Imports System.Net.Sockets
Imports System.Text.RegularExpressions

Public Class Form1
    Dim reading As Boolean = False
    Dim title As String = ""
    Dim chapter As String = ""
    Dim subtitle As String = ""
    Dim honbun As String = ""
    Dim nextStory As String = ""
    Dim startpage As String = ""
    Dim start As Int32
    Dim length As Int32
    Dim oldStart As Int32
    Dim bouyomiError As Boolean = False
    Dim homeUrl As String = "http://syosetu.com/"
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        bouyomicheck()
        Me.ClientSize = My.Settings.MyClientSize
        startpage = My.Settings.LastUrl
        start = My.Settings.LastSelection
        length = 0
        oldStart = 0
        WebBrowser1.ScriptErrorsSuppressed = True

        If startpage.Length = 0 Then startpage = homeUrl
        loadURL(startpage)

    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
        WebBrowser1.GoBack()
    End Sub

    Private Sub WebBrowser1_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser1.DocumentCompleted
        ProgressBar1.Hide()
        Dim content As HtmlElement = Nothing
        Dim origHtml = ""
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
        content = Doc.GetElementById("novel_honbun")
        If content IsNot Nothing Then
            origHtml = content.InnerHtml
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
                    title = el.InnerText + ControlChars.NewLine
                End If
            Next

            Dim ps As HtmlElementCollection = Doc.GetElementsByTagName("P")
            For Each el As HtmlElement In ps
                Dim eclass As String = el.GetAttribute("className")
                If eclass = "novel_subtitle" Then
                    subtitle = el.InnerText + ControlChars.NewLine

                ElseIf eclass = "chapter_title" Then
                    chapter = el.InnerText + ControlChars.NewLine

                End If
            Next



            Dim elems As HtmlElementCollection
            Dim rubyChanged As Boolean = False
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
                        If Regex.IsMatch(yomi.InnerText, "[^,.、。．・]") Then
                            isBouten = False
                        End If
                        yomiText = yomiText + yomi.InnerText
                    Next
                    If isBouten Then
                        For Each yomi As HtmlElement In rtcol
                            yomi.InnerHtml = ""
                        Next
                    Else
                        elem.OuterHtml = yomiText
                    End If


                Next
                honbun = content.InnerText

                content.InnerHtml = origHtml
            Else
                honbun = content.InnerText
            End If
            honbun = title + subtitle + honbun
        Else
            honbun = ""
        End If
        'Catch
        '    honbun = ""
        'End Try
        TextBox1.Text = honbun
        If honbun.Length > 0 Then

            TextBox1.Select()
            TextBox1.SelectionStart = 0
            TextBox1.SelectionLength = 0
            TextBox1.Select(start, 0)
            TextBox1.ScrollToCaret()
            'start = 0
            'length = 0


            Timer1.Interval = 100
            Timer1.Start()
        End If
        ProgressBar1.Hide()
    End Sub
    Private Sub bouyomicheck()
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
            MessageBox.Show("棒読みちゃんが起動していないため、読み上げができません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

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
            'MessageBox.Show("棒読みちゃんと通信できません", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
        Timer1.Stop()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        start = TextBox1.SelectionStart
        oldStart = start
        length = 0
        Timer1.Interval = 100
        Timer1.Start()
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
        Timer1.Stop()
        stopbouyomi()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If isTalking() Then
            TextBox1.Select(oldStart, start - oldStart + 1)
            TextBox1.ScrollToCaret()
            Return
        End If

        Dim src As String = TextBox1.Text
        If (TextBox1.Text.Length < 1) Then
            reading = False
            Timer1.Stop()
            Return
        End If
        Dim lineend As Int32
        Try
            lineend = TextBox1.Text.IndexOf(ControlChars.NewLine, start)
        Catch
            lineend = start
        End Try
        If lineend = -1 Then
            lineend = TextBox1.Text.Length
        End If
        length = lineend - start
        TextBox1.SelectionStart = start
        If length >= 0 Then
            Try
                src = src.Substring(start, length)
            Catch
                src = src.Substring(start - 1, 0)
            End Try
        End If

        If src.Length > 0 Then
            TextBox1.Select(start, length)

            bouyomi(src)
        End If
        TextBox1.ScrollToCaret()
        oldStart = start
        start = start + length + 1

        If start >= TextBox1.Text.Length Then
            Timer1.Stop()
            Dim nexturl As String = nextStory
            If nextStory.Length > 0 Then
                loadURL(nextStory)
            Else
                start = TextBox1.Text.Length
            End If
        End If

    End Sub


    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        My.Settings.MyClientSize = Me.ClientSize
        My.Settings.LastUrl = TextBox_url.Text
        My.Settings.LastSelection = start
    End Sub
    Private Sub loadURL(url As String)
        TextBox_url.Text = url
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


End Class
