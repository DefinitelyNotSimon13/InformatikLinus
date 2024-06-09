Imports System.IO
Imports Microsoft.VisualBasic.FileIO

' Einlesen und Speichern der Punktzahlen
Module ScoreboardLogik
    Public Sub SpeicherePunktzahl(ByRef name As String, ByVal charakter As Charaktere, ByVal punktzahl As Integer, ByRef schwierigkeit As Schwierigkeiten)
        My.Computer.FileSystem.WriteAllText(RufeScoreboardDateiAb(), """" + punktzahl.ToString + """,""" + name + """,""" + charakter.ToString + """,""" + schwierigkeit.ToString + """" + vbCrLf, True)
    End Sub

    Public Function LeseScoreboardEin() As List(Of Tuple(Of String, String, String, String))
        ' Aufbau des Tuples: Punktzahl, Name, Charakter, Schwiereigkeit
        Dim eintraege As New List(Of Tuple(Of String, String, String, String))
        Dim felder As String() ' Felder in einer Zeile

        Using parser As New TextFieldParser(RufeScoreboardDateiAb())
            parser.TextFieldType = FieldType.Delimited
            ' Felder in Zeilen sind Komma getrennt
            parser.SetDelimiters(","c)
            While Not parser.EndOfData
                Try
                    felder = parser.ReadFields()
                Catch e As MalformedLineException
                    Debug.WriteLine("Line " + e.LineNumber.ToString + " can not be parsed: " + e.Message)
                    Continue While
                End Try
                eintraege.Add(New Tuple(Of String, String, String, String)(felder(0), felder(1), felder(2), felder(3)))
            End While
        End Using

        ' Sortieren nach Punktzahl
        eintraege = eintraege.OrderByDescending(Function(t) Integer.Parse(t.Item1)).ToList
        Return eintraege
    End Function

    Function RufeScoreboardDateiAb() As String
        Dim projektVerzeichnis As String = Directory.GetParent(My.Application.Info.DirectoryPath).Parent.FullName
        Dim scoreboardVerzeichnis As String = Path.Combine(projektVerzeichnis, "resources/")
        Return scoreboardVerzeichnis + "highscore.csv"
    End Function

End Module
