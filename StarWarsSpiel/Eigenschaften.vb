' Eigenschaften die an verschiedenen Stellen des Programms verwendet werden
Module Eigenschaften

    Public Property MinGroesseHinderniss As Integer

    Public Property MaxGroesseHinderniss As Integer

    Public Property Schwierigkeit As Schwierigkeiten

    Public Property Charakter As Charaktere

    Public Property Name As String = "No Name"

    Public Property Multiplayer As Boolean = False '


    ' Eigenschaft zur Abfrage der Tastatureingabe
    ' -- Ist letztendlich ähnlich zu einer Funktion aber einfach etwas cleaner in dem Kontext
    ReadOnly Property TastaturAbfrage As Tasten
        Get
            If Console.KeyAvailable = False Then
                Return Tasten.None
            End If
            Select Case Console.ReadKey(True).Key
                Case ConsoleKey.LeftArrow
                    Return Tasten.Left
                Case ConsoleKey.RightArrow
                    Return Tasten.Right
                Case ConsoleKey.Spacebar
                    Return Tasten.Space
                Case ConsoleKey.UpArrow
                    Return Tasten.Up
                Case ConsoleKey.DownArrow
                    Return Tasten.Down
                Case ConsoleKey.P
                    Return Tasten.P
                Case ConsoleKey.W
                    Return Tasten.W
                Case ConsoleKey.A
                    Return Tasten.A
                Case ConsoleKey.S
                    Return Tasten.S
                Case ConsoleKey.D
                    Return Tasten.D
                Case ConsoleKey.Y
                    Return Tasten.Y
                Case ConsoleKey.V
                    Return Tasten.V
                Case ConsoleKey.Enter
                    Return Tasten.Enter
                Case Else
                    Return Tasten.Unknown
            End Select
        End Get
    End Property
End Module
