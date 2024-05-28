Imports System.IO
Imports System.IO.Ports

Public Class Form1
    Private WithEvents SerialPort1 As New SerialPort()

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Initialize the serial port
        SerialPort1.PortName = "COM3"
        SerialPort1.BaudRate = 115200 'this baudrate is specifaccly for arduino uno r4 wifi
        SerialPort1.DataBits = 8
        SerialPort1.Parity = Parity.None
        SerialPort1.StopBits = StopBits.One
        SerialPort1.Handshake = Handshake.None
        SerialPort1.DtrEnable = True

        AddHandler SerialPort1.DataReceived, AddressOf SerialPort1_DataReceived

        ' Open the serial port
        Try
            SerialPort1.Open()
            Console.WriteLine("Serial port opened successfully.")
        Catch ex As UnauthorizedAccessException
            MessageBox.Show("Access to the port is denied. Please ensure no other application is using the port and try running the application as an administrator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As IOException
            MessageBox.Show("An I/O error occurred while accessing the port. Please ensure the port is available and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' Close the serial port when the form is closing
        If SerialPort1 IsNot Nothing AndAlso SerialPort1.IsOpen Then
            SerialPort1.Close()
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Send data to the Arduino 
        If SerialPort1 IsNot Nothing AndAlso SerialPort1.IsOpen Then
            SerialPort1.WriteLine(TextBox1.Text)
            Threading.Thread.Sleep(100)
        End If
    End Sub

    Private Sub SerialPort1_DataReceived(sender As Object, e As SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived
        ' Read data from the Arduino
        Try
            Dim incomingData As String = SerialPort1.ReadLine().Trim()

            Threading.Thread.Sleep(100)
            Const R2 As Double = 200000.0
            Const Vin As Double = 12.0
            Const Vref As Double = 3.0
            Dim Vout As Double

            If Double.TryParse(incomingData, Vout) Then
                If Vout > 3.0 Then
                    Me.Invoke(New MethodInvoker(Sub()
                                                    TextBox2.AppendText("Vout: " & Vout & " V" & Environment.NewLine)
                                                    TextBox2.AppendText("Result: " & Vref & Environment.NewLine)
                                                End Sub))
                Else
                    Dim result As Double = calculateRdut(Vin, Vref, Vout, R2)
                    Me.Invoke(New MethodInvoker(Sub()
                                                    TextBox2.AppendText("Vout: " & Vout & " V" & Environment.NewLine)
                                                    TextBox2.AppendText("Calculated Rdut: " & result & " ohms" & Environment.NewLine)
                                                End Sub))
                End If
            Else
                Me.Invoke(New MethodInvoker(Sub()
                                                TextBox2.AppendText("Invalid data received: " & incomingData & Environment.NewLine)
                                            End Sub))
            End If
        Catch ex As Exception
            MessageBox.Show("Error reading data: " & ex.Message)
        End Try
    End Sub
    Function calculateRdut(Vin As Double, Vref As Double, Vout As Double, R2 As Double) As Double
        Return (R2 * (Vin - Vref)) / (Vref - Vout)
    End Function


    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged
    End Sub
End Class


