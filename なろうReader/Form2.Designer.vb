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
        Me.CheckBox_autoRead = New System.Windows.Forms.CheckBox()
        Me.CheckBox_autoNext = New System.Windows.Forms.CheckBox()
        Me.FontDialog1 = New System.Windows.Forms.FontDialog()
        Me.Button_Font = New System.Windows.Forms.Button()
        Me.FontLabel = New System.Windows.Forms.Label()
        Me.OKButton = New System.Windows.Forms.Button()
        Me.Button_Cancel = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBox_bPath = New System.Windows.Forms.TextBox()
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
        Me.Button_Font.Location = New System.Drawing.Point(12, 72)
        Me.Button_Font.Name = "Button_Font"
        Me.Button_Font.Size = New System.Drawing.Size(92, 23)
        Me.Button_Font.TabIndex = 2
        Me.Button_Font.Text = "フォント設定…"
        Me.Button_Font.UseVisualStyleBackColor = True
        '
        'FontLabel
        '
        Me.FontLabel.AutoSize = True
        Me.FontLabel.Location = New System.Drawing.Point(110, 77)
        Me.FontLabel.Name = "FontLabel"
        Me.FontLabel.Size = New System.Drawing.Size(38, 12)
        Me.FontLabel.TabIndex = 3
        Me.FontLabel.Text = "Label1"
        '
        'OKButton
        '
        Me.OKButton.Location = New System.Drawing.Point(235, 168)
        Me.OKButton.Name = "OKButton"
        Me.OKButton.Size = New System.Drawing.Size(75, 23)
        Me.OKButton.TabIndex = 4
        Me.OKButton.Text = "OK"
        Me.OKButton.UseVisualStyleBackColor = True
        '
        'Button_Cancel
        '
        Me.Button_Cancel.Location = New System.Drawing.Point(324, 168)
        Me.Button_Cancel.Name = "Button_Cancel"
        Me.Button_Cancel.Size = New System.Drawing.Size(75, 23)
        Me.Button_Cancel.TabIndex = 5
        Me.Button_Cancel.Text = "Cancel"
        Me.Button_Cancel.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(335, 128)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(64, 23)
        Me.Button1.TabIndex = 6
        Me.Button1.Text = "探す..."
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 114)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(101, 12)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "棒読みちゃんの場所"
        '
        'TextBox_bPath
        '
        Me.TextBox_bPath.Location = New System.Drawing.Point(14, 130)
        Me.TextBox_bPath.Name = "TextBox_bPath"
        Me.TextBox_bPath.ReadOnly = True
        Me.TextBox_bPath.Size = New System.Drawing.Size(315, 19)
        Me.TextBox_bPath.TabIndex = 8
        '
        'Form2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(411, 202)
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
        Me.Location = Global.なろうReader.My.MySettings.Default.SettingLocation
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(427, 241)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(427, 241)
        Me.Name = "Form2"
        Me.ShowIcon = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.Text = "なろうReader 設定"
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents CheckBox_autoRead As CheckBox
    Friend WithEvents CheckBox_autoNext As CheckBox
    Friend WithEvents FontDialog1 As FontDialog
    Friend WithEvents Button_Font As Button
    Friend WithEvents FontLabel As Label
    Friend WithEvents OKButton As Button
    Friend WithEvents Button_Cancel As Button
    Friend WithEvents Button1 As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents TextBox_bPath As TextBox
End Class
