Imports System.Net.Sockets
Imports System.Threading
Imports System.Net
Imports System.Text

Public Class Form1
    Dim receivingClient As UdpClient
    Dim receivingThread As Thread
    Dim timeDown_msg As String
    Private Delegate Sub WriteMessageDelegate(ByVal msg As String)

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        'CheckForIllegalCrossThreadCalls = False
        InitializeReceiver()
        Call form1_Resize()
    End Sub

    Private Sub InitializeReceiver()
        Try
            receivingClient = New UdpClient(1234)
            Dim start As ThreadStart = New ThreadStart(AddressOf Receiver)
            receivingThread = New Thread(start)
            receivingThread.IsBackground = True
            receivingThread.Start()
        Catch ex As Exception
            MsgBox("Port 1234 is already in use", vbExclamation, "Port in already use")
        End Try
    End Sub

    Private Sub Receiver()
        Dim endPoint As IPEndPoint = New IPEndPoint(IPAddress.Any, 1234)
        While (True)
            Dim data() As Byte
            data = receivingClient.Receive(endPoint)
            Dim msg As String = Encoding.ASCII.GetString(data)
            WriteMessage(msg)
        End While
    End Sub

    Private Sub WriteMessage(ByVal msg As String)
        Try
            If LabelPastTime.InvokeRequired and msg.Length = 8 Then
                Dim d As New WriteMessageDelegate(AddressOf WriteMessage)
                LabelPastTime.Invoke(d, New Object() {msg})
            Else
                Timer1.Start()
                LabelPastTime.ForeColor = Color.White
                LabelPastTime.Text = msg
            End If
        Catch ex As Exception
            Timer1.Stop()
            MsgBox("Bad time format. Use hh:mm:ss" & vbCrLf & "Example 02:08:05", vbExclamation, "Bad time format 1")
            Return
        End Try
    End Sub


    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Timer1.Interval = 1000
        Try
            timeDown_msg = FormatDateTime(DateAdd(DateInterval.Second, -1, DateTime.Parse(LabelPastTime.Text)), DateFormat.LongTime)
            LabelPastTime.Text = timeDown_msg
            If timeDown_msg = "00:00:00" Or timeDown_msg = "23:59:59" Then
                Timer1.Stop()
                timeDown_msg = "00:00:00"
            End If
        Catch ex As Exception
            Timer1.Stop()
            MsgBox("Bad time format. Use hh:mm:ss" & vbCrLf & "Example 02:08:05", vbExclamation, "Bad time format")
            LabelPastTime.Text = "00:00:00"
        End Try
    End Sub

    Private Sub Label2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label2.Click
        LabelPastTime.Text = "00:00:00"
        Timer1.Stop()
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles Me.FormClosing
        End
    End Sub


    Sub form1_Resize() Handles Me.Resize
        Dim PastTimeFontSize As Single
        PastTimeFontSize = 2 * (Me.Width * 0.07)
        LabelPastTime.Font = New Font("Microsoft Tai Le", PastTimeFontSize, FontStyle.Bold)
        LabelPastTime.Location = New Point(LabelPastTime.Parent.Width \ 2 - LabelPastTime.Width \ 2, LabelPastTime.Parent.Height \ 2 - LabelPastTime.Height * 0.4)

        Dim Label1FontSize As Single
        Label1FontSize = 0.1 * (Me.Width \ 2)
        Label1.Font = New Font("Microsoft Tai Le", Label1FontSize, FontStyle.Bold)

        Label1.Location = New Point(Label1.Parent.Width \ 2 - Label1.Width \ 2, 0)
    End Sub


End Class


