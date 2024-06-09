Public Class Spiel
    Private lebenSpieler1 As Integer ' verbleibende Leben
    Private lebenSpieler2 As Integer ' verbleibende Leben
    Private ReadOnly spielfeld(ZEILE_MAX_ANZAHL, SPALTE_MAX_ANZAHL) As Char
    Private zeile(SPALTE_MAX_ANZAHL) As Char ' neueste Zeile

    ' Position Spieler 1
    Private spieler1Spalte As Integer
    Private spieler1Zeile As Integer

    ' Position Spieler 2
    Private spieler2Spalte As Integer
    Private spieler2Zeile As Integer

    Private schussZaehler As Integer = 0 ' Zu Beginn 0 Schuesse
    Private _punktzahl As Integer = 0 ' Korrektur der Punktzahl um -200 spaeter

    Public ReadOnly Property Punktzahl As Integer
        Get
            Return _punktzahl
        End Get
    End Property

    Public Sub Initialisiere()
        ' Anfangsleben setzen:
        If Charakter = Charaktere.Yoda Then
            lebenSpieler1 = 5
            lebenSpieler2 = 5
        ElseIf Charakter = Charaktere.DarthVader Then
            lebenSpieler1 = 3
            lebenSpieler2 = 3
        End If

        ' Startposition Spieler 1
        spieler1Spalte = SPALTE_MAX_ANZAHL / 2
        spieler1Zeile = ZEILE_MAX_ANZAHL

        ' Startposition Spieler 2 (Multiplayer)
        If Multiplayer Then
            spieler2Spalte = SPALTE_MAX_ANZAHL / 4
            spieler2Zeile = ZEILE_MAX_ANZAHL
        End If
    End Sub


    Public Sub SpielfigurZuruecksetzen()
        'Alte Spielfigur1 löschen:
        Console.SetCursorPosition(spieler1Spalte, spieler1Zeile)
        Console.Write(" ")

        'Alte Spielfigur2 löschen (Multiplayer):
        If Multiplayer Then
            Console.SetCursorPosition(spieler2Spalte, spieler2Zeile)
            Console.Write(" ")
        End If
    End Sub

    Sub ErzeugeGadgets(ByVal zaehler As Integer)

        'Neue Herzen alle drei Zeilen erzeugen:
        If zaehler Mod 3 = 0 Then
            ErzeugeHerz(zeile)
        End If

        '+5 Punkte alle 2 Zeilen erzeugen:
        If zaehler Mod 2 = 0 Then
            Erzeuge5Punkte(zeile)
        End If

        '*2 Punkte alle 15 Zeilen erzeugen:
        If zaehler Mod 15 = 0 Then
            Erzeuge2Punkte(zeile)
        End If

        '+Schuss alle 2 Zeilen erzeugen (nur Singleplayer):
        If zaehler Mod 2 = 0 AndAlso Not Multiplayer Then
            ErzeugeSchussDrop(zeile)
        End If
    End Sub

    Public Sub VerschiebeSpielfeld(ByVal hindernisMaxAnzahl As Integer, ByVal zaehler As Integer)
        ' Neue Zeile und darin Gadgets erzeugen
        zeile = ErzeugeZeile(hindernisMaxAnzahl)
        ErzeugeGadgets(zaehler)

        'Hindernisse um eine Zeile nach unten verschieben:
        For z = ZEILE_MAX_ANZAHL To 1 Step -1
            For s = 0 To SPALTE_MAX_ANZAHL
                spielfeld(z, s) = spielfeld(z - 1, s)
            Next
        Next

        'Neue Zeile in die erste Zeile des Spielfelds übertragen:
        For s = 0 To SPALTE_MAX_ANZAHL
            spielfeld(0, s) = zeile(s)
        Next
    End Sub

    Public Sub SpielfeldAusgeben()
        Console.SetCursorPosition(0, 0)

        'Farben der Gadgets ändern:
        For z = 0 To 23
            For s = 0 To SPALTE_MAX_ANZAHL
                Select Case spielfeld(z, s)
                    Case "|"
                        Console.ForegroundColor = ConsoleColor.DarkYellow
                    Case "2"
                        Console.ForegroundColor = ConsoleColor.White
                    Case "5"
                        Console.ForegroundColor = ConsoleColor.Cyan
                    Case "♥"
                        If Charakter = Charaktere.Yoda Then
                            Console.ForegroundColor = ConsoleColor.DarkGreen
                        ElseIf Charakter = Charaktere.DarthVader Then
                            Console.ForegroundColor = ConsoleColor.DarkRed
                        End If
                    Case Else
                        'Farbe je nach Charakter anpassen:
                        If Charakter = Charaktere.Yoda Then
                            Console.ForegroundColor = ConsoleColor.DarkRed
                        ElseIf Charakter = Charaktere.DarthVader Then
                            Console.ForegroundColor = ConsoleColor.Green
                        End If
                End Select

                Console.Write(spielfeld(z, s))
            Next
            Console.WriteLine()
        Next
    End Sub

    Public Sub TastenAktionDurchführen(taste As Integer)
        ' Aktion je nach Taste durchführen
        Select Case taste
            Case Tasten.Left
                spieler1Spalte -= 1
                If spieler1Spalte < 0 Then spieler1Spalte = 0
            Case Tasten.Right
                spieler1Spalte += 1
                If spieler1Spalte > SPALTE_MAX_ANZAHL Then spieler1Spalte = SPALTE_MAX_ANZAHL
            Case Tasten.Up
                spieler1Zeile -= 1
                If spieler1Zeile < 20 Then spieler1Zeile = 20
            Case Tasten.Down
                spieler1Zeile += 1
                If spieler1Zeile > ZEILE_MAX_ANZAHL Then spieler1Zeile = ZEILE_MAX_ANZAHL
            Case Tasten.W
                spieler2Zeile -= 1
                If spieler2Zeile < 20 Then spieler2Zeile = 20
            Case Tasten.A
                spieler2Spalte -= 1
                If spieler2Spalte < 0 Then spieler2Spalte = 0
            Case Tasten.S
                spieler2Zeile += 1
                If spieler2Zeile > ZEILE_MAX_ANZAHL Then spieler2Zeile = ZEILE_MAX_ANZAHL
            Case Tasten.D
                spieler2Spalte += 1
                If spieler2Spalte > SPALTE_MAX_ANZAHL Then spieler2Spalte = SPALTE_MAX_ANZAHL
            Case Tasten.Space
                If schussZaehler > 0 Then
                    SchussAbfeuern()
                End If
        End Select
    End Sub

    Public Sub SpielfigurAusgeben()
        If Not Multiplayer Then
            Console.SetCursorPosition(spieler1Spalte, spieler1Zeile)
            'Wenn Yoda der Charakter ist:
            If Charakter = Charaktere.Yoda Then
                Console.ForegroundColor = ConsoleColor.Green
                Console.Write("=")
                'Wenn Darth Vader der Charakter ist:
            ElseIf Charakter = Charaktere.DarthVader Then
                Console.ForegroundColor = ConsoleColor.DarkRed
                Console.Write("!")
            End If

            'Spielfigur Multiplayer ausgeben:
        ElseIf Multiplayer Then
            'Wenn Yoda der Charakter ist:
            If Charakter = Charaktere.Yoda Then
                Console.ForegroundColor = ConsoleColor.Green
                Console.SetCursorPosition(spieler1Spalte, spieler1Zeile)
                Console.Write("1")
                Console.SetCursorPosition(spieler2Spalte, spieler2Zeile)
                Console.Write("2")
                'Wenn Darth Vader der Charakter ist:
            ElseIf Charakter = Charaktere.DarthVader Then
                Console.ForegroundColor = ConsoleColor.DarkRed
                Console.SetCursorPosition(spieler1Spalte, spieler1Zeile)
                Console.Write("1")
                Console.SetCursorPosition(spieler2Spalte, spieler2Zeile)
                Console.Write("2")
            End If
        End If
    End Sub

    Public Sub KollisionsPruefungSchuss()
        If spielfeld(spieler1Zeile - 1, spieler1Spalte) = "|" Then
            schussZaehler += 1
            spielfeld(spieler1Zeile - 1, spieler1Spalte) = " "
        End If
    End Sub


    Private Sub SchussAbfeuern()
        Console.ForegroundColor = ConsoleColor.Blue
        If Charakter = Charaktere.Yoda Then
            For k = 1 To 5
                Console.SetCursorPosition(spieler1Spalte, spieler1Zeile - k)
                Console.WriteLine("|")
                'Thread.Sleep(10)
            Next
        ElseIf Charakter = Charaktere.DarthVader Then
            For k = 1 To 6
                Console.SetCursorPosition(spieler1Spalte, spieler1Zeile - k)
                Console.WriteLine("|")
                'Thread.Sleep(10)
            Next
        End If

        SpieleTonUndDanachSchleife("Blaster", "ImperialMarch")

        schussZaehler -= 1

        'Kollisionsprüfung nach russischer Methode:
        If spielfeld(spieler1Zeile - 1, spieler1Spalte) = "■" Then
            spielfeld(spieler1Zeile - 1, spieler1Spalte) = " "
        End If
        If spielfeld(spieler1Zeile - 2, spieler1Spalte) = "■" Then
            spielfeld(spieler1Zeile - 2, spieler1Spalte) = " "
        End If
        If spielfeld(spieler1Zeile - 3, spieler1Spalte) = "■" Then
            spielfeld(spieler1Zeile - 3, spieler1Spalte) = " "
        End If
        If spielfeld(spieler1Zeile - 4, spieler1Spalte) = "■" Then
            spielfeld(spieler1Zeile - 4, spieler1Spalte) = " "
        End If
        If spielfeld(spieler1Zeile - 5, spieler1Spalte) = "■" Then
            spielfeld(spieler1Zeile - 5, spieler1Spalte) = " "
        End If
        'Nur Bei Darth Vader wird das 6. Feld über der Spielfigur gelöscht:
        If Charakter = Charaktere.DarthVader And spielfeld(spieler1Zeile - 6, spieler1Spalte) = "■" Then
            spielfeld(spieler1Zeile - 6, spieler1Spalte) = " "
        End If
    End Sub

    Public Sub SchussAnzahlAusgeben()
        Console.ForegroundColor = ConsoleColor.Blue
        Console.SetCursorPosition(26, 26)
        Console.WriteLine("Verbleibende Schüsse: " & schussZaehler)
    End Sub

    Public Sub SpielPausieren()
        Dim taste As Tasten
        Console.Clear()
        SpieleTonImHintergrundAb("Wartemusik")
        Console.ForegroundColor = ConsoleColor.DarkYellow
        Console.SetCursorPosition(0, 10)
        Console.WriteLine("                                         Das Spiel ist pausiert")
        Console.WriteLine("                                      P drücken um weiterzuspielen")
        Do
            taste = TastaturAbfrage
        Loop Until taste = Tasten.P

        Console.Clear()
        If Charakter = "Yoda" Then
            ' -- Imperial March bei beiden?
            My.Computer.Audio.Play("ImperialMarch.wav", AudioPlayMode.BackgroundLoop)
        ElseIf Charakter = "Darth Vader" Then
            My.Computer.Audio.Play("ImperialMarch.wav", AudioPlayMode.BackgroundLoop)
        End If
    End Sub

    Public Sub KollisionPruefenSpieler1()
        ' KollisionPruefen gibt -1 bei Hindernis und 1 bei Leben zurück
        lebenSpieler1 += KollisionPruefen(spielfeld(spieler1Zeile, spieler1Spalte))
        spielfeld(spieler1Zeile, spieler1Spalte) = " "
    End Sub

    Public Sub KollisionPruefenSpieler2()
        ' KollisionPruefen gibt -1 bei Hindernis und 1 bei Leben zurück
        lebenSpieler2 += KollisionPruefen(spielfeld(spieler2Zeile, spieler2Spalte))
        spielfeld(spieler2Zeile, spieler2Spalte) = " "
    End Sub

    Private Function KollisionPruefen(ByVal symbol As Char) As Integer
        Select Case symbol
            Case "■"
                Sound.SpieleTonUndDanachSchleife("Tod", "ImperialMarch")
                Return -1
            Case "♥"
                Return 1
            Case "5"
                _punktzahl += 5
            Case "2"
                _punktzahl *= 2
        End Select
        Return 0
    End Function

    Public Sub KollisionZwichenSpielernPruefen()
        If Multiplayer Then
            If spieler2Zeile = "1" Then
                spieler2Zeile -= 1
            End If
            If spielfeld(spieler1Zeile, spieler1Spalte) = "2" Then
                spielfeld(spieler1Zeile, spieler1Spalte) = spielfeld(spieler1Zeile - 1, spieler1Spalte - 1)
            End If
        End If

    End Sub

    Public Sub LebenAusgebenSpieler1()
        If Charakter = Charaktere.Yoda Then
            Console.ForegroundColor = ConsoleColor.Green
        ElseIf Charakter = Charaktere.DarthVader Then
            Console.ForegroundColor = ConsoleColor.DarkRed
        End If
        Console.SetCursorPosition(50, 25)
        Console.Write("Leben: ")
        For u As Integer = 1 To lebenSpieler1
            Console.Write("♥ ")
        Next
        Console.Write("  ")
    End Sub

    Public Sub LebenAusgebenSpieler2()
        If Multiplayer Then
            Console.SetCursorPosition(0, 25)
            Console.Write("Leben: ")
            For u = 1 To lebenSpieler2
                Console.Write("♥ ")
            Next
            Console.Write("  ")
        End If
    End Sub

    Public Sub PunktzahlErhoehen(ByVal wert As Integer)
        _punktzahl += 10
    End Sub

    Public Sub PunktzahlAnzeigen()
        Console.SetCursorPosition(33, 25)
        Console.WriteLine("Score: ")
        Dim korrigiertePunktzahl = Punktzahl - 200
        Debug.WriteLine("Punktzahl: " + korrigiertePunktzahl.ToString)
        If korrigiertePunktzahl < 0 Then
            Console.SetCursorPosition(40, 25)
            Console.Write("0")
        ElseIf korrigiertePunktzahl > 0 Then
            Console.SetCursorPosition(40, 25)
            Console.Write(korrigiertePunktzahl)
        End If

    End Sub

    Public Function SpielerIstGestorben() As Boolean
        Return lebenSpieler1 <= 0 Or lebenSpieler2 <= 0
    End Function


End Class
