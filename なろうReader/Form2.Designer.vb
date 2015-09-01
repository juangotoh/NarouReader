<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form2
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.CheckBox_autoRead = New System.Windows.Forms.CheckBox()
        Me.CheckBox_autoNext = New System.Windows.Forms.CheckBox()
        Me.Button_Font = New System.Windows.Forms.Button()
        Me.FontLabel = New System.Windows.Forms.Label()
        Me.OKButton = New System.Windows.Forms.Button()
        Me.Button_Cancel = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBox_bPath = New System.Windows.Forms.TextBox()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.LinkLabel1 = New System.Windows.Forms.LinkLabel()
        Me.FontDialog1 = New System.Windows.Forms.FontDialog()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.CheckBox_Maegaki = New System.Windows.Forms.CheckBox()
        Me.CheckBox_Atogaki = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'CheckBox_autoRead
        '
        Me.CheckBox_autoRead.AutoSize = True
        Me.CheckBox_autoRead.Location = New System.Drawing.Point(12, 12)
        Me.CheckBox_autoRead.Name = "CheckBox_autoRead"
        Me.CheckBox_autoRead.Size = New System.Drawing.Size(252, 16)
        Me.CheckBox_autoRead.TabIndex = 0
        Me.CheckBox_autoRead.Text = "ページ読み込み時、自動的に小説を読み上げる"
        Me.CheckBox_autoRead.UseVisualStyleBackColor = True
        '
        'CheckBox_autoNext
        '
        Me.CheckBox_autoNext.AutoSize = True
        Me.CheckBox_autoNext.Location = New System.Drawing.Point(12, 34)
        Me.CheckBox_autoNext.Name = "CheckBox_autoNext"
        Me.CheckBox_autoNext.Size = New System.Drawing.Size(252, 16)
        Me.CheckBox_autoNext.TabIndex = 1
        Me.CheckBox_autoNext.Text = "連載作品の続きがある場合、自動的に移動する"
        Me.CheckBox_autoNext.UseVisualStyleBackColor = True
        '
        'Button_Font
        '
        Me.Button_Font.Location = New System.Drawing.Point(13, 111)
        Me.Button_Font.Name = "Button_Font"
        Me.Button_Font.Size = New System.Drawing.Size(92, 23)
        Me.Button_Font.TabIndex = 2
        Me.Button_Font.Text = "フォント設定…"
        Me.ToolTip1.SetToolTip(Me.Button_Font, "画面下部の「読み上げ用テキストエリア」に表示する文字の書体を設定します。")
        Me.Button_Font.UseVisualStyleBackColor = True
        '
        'FontLabel
        '
        Me.FontLabel.AutoSize = True
        Me.FontLabel.Location = New System.Drawing.Point(109, 116)
        Me.FontLabel.Name = "FontLabel"
        Me.FontLabel.Size = New System.Drawing.Size(38, 12)
        Me.FontLabel.TabIndex = 3
        Me.FontLabel.Text = "Label1"
        '
        'OKButton
        '
        Me.OKButton.Location = New System.Drawing.Point(234, 207)
        Me.OKButton.Name = "OKButton"
        Me.OKButton.Size = New System.Drawing.Size(75, 23)
        Me.OKButton.TabIndex = 4
        Me.OKButton.Text = "OK"
        Me.OKButton.UseVisualStyleBackColor = True
        '
        'Button_Cancel
        '
        Me.Button_Cancel.Location = New System.Drawing.Point(323, 207)
        Me.Button_Cancel.Name = "Button_Cancel"
        Me.Button_Cancel.Size = New System.Drawing.Size(75, 23)
        Me.Button_Cancel.TabIndex = 5
        Me.Button_Cancel.Text = "Cancel"
        Me.Button_Cancel.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(334, 167)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(64, 23)
        Me.Button1.TabIndex = 6
        Me.Button1.Text = "探す..."
        Me.ToolTip1.SetToolTip(Me.Button1, "棒読みちゃんの実行ファイルがある場所を設定します。ファイル名はBouyuomiChan.exeです。")
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(11, 153)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(233, 12)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "棒読みちゃんの実行ファイル(BouyomiChan.exe)"
        '
        'TextBox_bPath
        '
        Me.TextBox_bPath.Location = New System.Drawing.Point(13, 169)
        Me.TextBox_bPath.Name = "TextBox_bPath"
        Me.TextBox_bPath.Size = New System.Drawing.Size(315, 19)
        Me.TextBox_bPath.TabIndex = 8
        Me.ToolTip1.SetToolTip(Me.TextBox_bPath, "棒読みちゃんの実行ファイルがある場所を設定します。ファイル名はBouyuomiChan.exeです。")
        '
        'LinkLabel1
        '
        Me.LinkLabel1.AutoSize = True
        Me.LinkLabel1.Location = New System.Drawing.Point(9, 207)
        Me.LinkLabel1.Name = "LinkLabel1"
        Me.LinkLabel1.Size = New System.Drawing.Size(152, 12)
        Me.LinkLabel1.TabIndex = 9
        Me.LinkLabel1.TabStop = True
        Me.LinkLabel1.Text = "棒読みちゃん配布サイトへ移動"
        '
        'FontDialog1
        '
        Me.FontDialog1.AllowVerticalFonts = False
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'CheckBox_Maegaki
        '
        Me.CheckBox_Maegaki.AutoSize = True
        Me.CheckBox_Maegaki.Location = New System.Drawing.Point(12, 56)
        Me.CheckBox_Maegaki.Name = "CheckBox_Maegaki"
        Me.CheckBox_Maegaki.Size = New System.Drawing.Size(88, 16)
        Me.CheckBox_Maegaki.TabIndex = 10
        Me.CheckBox_Maegaki.Text = "前書きを読む"
        Me.ToolTip1.SetToolTip(Me.CheckBox_Maegaki, "前書きがある場合、読み上げます。")
        Me.CheckBox_Maegaki.UseVisualStyleBackColor = True
        '
        'CheckBox_Atogaki
        '
        Me.CheckBox_Atogaki.AutoSize = True
        Me.CheckBox_Atogaki.Location = New System.Drawing.Point(12, 78)
        Me.CheckBox_Atogaki.Name = "CheckBox_Atogaki"
        Me.CheckBox_Atogaki.Size = New System.Drawing.Size(88, 16)
        Me.CheckBox_Atogaki.TabIndex = 11
        Me.CheckBox_Atogaki.Text = "後書きを読む"
        Me.ToolTip1.SetToolTip(Me.CheckBox_Atogaki, "後書きがある場合、読み上げます。")
        Me.CheckBox_Atogaki.UseVisualStyleBackColor = True
        '
        'Form2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(411, 241)
        Me.Controls.Add(Me.CheckBox_Atogaki)
        Me.Controls.Add(Me.CheckBox_Maegaki)
        Me.Controls.Add(Me.LinkLabel1)
        Me.Controls.Add(Me.TextBox_bPath)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.Button_Cancel)
        Me.Controls.Add(Me.OKButton)
        Me.Controls.Add(Me.FontLabel)
        Me.Controls.Add(Me.Button_Font)
        Me.Controls.Add(Me.CheckBox_autoNext)
        Me.Controls.Add(Me.CheckBox_autoRead)
        Me.DataBindings.Add(New System.Windows.Forms.Binding("Location", Global.なろうReader.My.MySettings.Default, "SettingLocation", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Location = Global.なろうReader.My.MySettings.Default.SettingLocation
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(427, 280)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(427, 280)
        Me.Name = "Form2"
        Me.ShowIcon = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.Text = "なろうReader 設定"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents CheckBox_autoRead As CheckBox
    Friend WithEvents CheckBox_autoNext As CheckBox
    Friend WithEvents Button_Font As Button
    Friend WithEvents FontLabel As Label
    Friend WithEvents OKButton As Button
    Friend WithEvents Button_Cancel As Button
    Friend WithEvents Button1 As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents TextBox_bPath As TextBox
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents LinkLabel1 As LinkLabel
    Friend WithEvents FontDialog1 As FontDialog
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents CheckBox_Maegaki As CheckBox
    Friend WithEvents CheckBox_Atogaki As CheckBox
End Class
