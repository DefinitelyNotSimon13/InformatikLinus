Imports System.Threading

' -- Die Größten Änderungen
' - Aufteilung auf mehrere Dateien:
'   - Main
'   - Eigenschaften - Globale Variablen auf welche über Eigenschaften zugegriffen werden kann
'   - Konstanten - Globale Konstanten und Enums
'   - ScoreboardLogik - Logik zum Speichern und Laden der Punktzahlen
'   - Sound - Funktionen zum Abspielen von Sounds
'   - SpielElemente - Erstellung von SpielElementen wie Hindernissen, Punkten oder der gesamten Zeile
'   - TextAusgabe - Laden von Textblöcken aus Dateien und wiedergeben von verzögerten 
' - Auslagerung der meisten Textblöcke in .txt Dateien
'
' Mit Änderung von einigen Variablen Namen haben sich meiner Meinung nach viele Kommentare erspart.
' Falls du stellen hast, die du nicht verstehst oder so, kann ich da noch welche hinzufügen
'
' Kommentare die mit -- Anfang sind von mir, also am besten löschen vor Abgabe haha


Module Main

    ' Hauptfunktion
    ' -- Nach Oben Bewegt für Übersicht
    Sub Main()
        Console.BackgroundColor = ConsoleColor.Black
        Console.SetWindowSize(115, 300)
        Console.Clear()
        Console.CursorVisible = False

        'GebeLadebalkenAus()
        'GebeNamenEin()
        'GebeVorspannAus()
        'GebeEroeffnungsTextAus()
        OeffneCharakterAuswahl(True)

        Console.ReadLine()
    End Sub


    'Hauptprozedur für den Spielablauf
    Sub HauptSchleife()
        ' -- Ich bin der Meinung dass die Variablen keine extra kommentare brauchen
        ' -- Kommentare sind schön und gut aber eigentlich sollte die Funktion der Variable schon
        ' -- im Namen stecken - dann führen Kommentare eher zu unschönerem Code
        Dim taste As Integer
        Dim wartezeit As Single = 500
        Dim hindernisMaxAnzahl As Integer = 5
        Dim hindernisZaehler As Integer
        Dim zaehler As Integer = 0

        Dim spiel As New Spiel() ' Spiel Klasse beinhaltet relevante Funktionen für den Spielfluss
        spiel.Initialisiere()


        Sound.SpieleTonImHintergrundAb("ImperialMarch")


        'Hauptschleife des Spiels:
        ' -- Der Code der vorher in der Schleife stand wurde effektiv einfach nur in die spiel Klasse ausgelagert
        Do

            ' Spielfeld nach unten verschieben und neue Zeile oben hinzufügen
            spiel.VerschiebeSpielfeld(hindernisMaxAnzahl, zaehler)


            spiel.SpielfeldAusgeben()

            ' Seperate Schleife für Spielfigur um Responsivität zu erhöhen
            For i = 1 To GESCHWINDIGKEIT_SPIELFIGUR

                spiel.SpielfigurZuruecksetzen()

                ' Schuss Kollisionsprüfung weiter vorne um Tod trotz abgegenen Schusses zu vermeiden
                spiel.KollisionsPruefungSchuss()

                ' Abfrage von Benutzer Eingabe
                taste = TastaturAbfrage
                spiel.TastenAktionDurchführen(taste)
                spiel.SpielfigurAusgeben()

                ' Im Multiplayer existieren keine Schüsse
                If Not Multiplayer Then
                    spiel.SchussAnzahlAusgeben()
                End If

                spiel.KollisionPruefenSpieler1()
                If Multiplayer Then
                    spiel.KollisionPruefenSpieler2()
                    spiel.KollisionZwichenSpielernPruefen()
                End If

                spiel.LebenAusgebenSpieler1()

                If Multiplayer Then
                    spiel.LebenAusgebenSpieler2()
                End If

                ' Wartezeit beinflusst Geschwindigkeit der Spielfigur
                Thread.Sleep(wartezeit / GESCHWINDIGKEIT_SPIELFIGUR)
            Next

            'Tastaturbuffer löschen:
            LeereTastaturBuffer()

            'Geschwindigkeit erhöhen:
            wartezeit *= 0.99

            'Hindernisdichte erhöhen:
            hindernisZaehler += 1
            If hindernisZaehler = 20 Then
                hindernisZaehler = 0
                hindernisMaxAnzahl += 1
            End If

            'Score wird angezeigt:
            'Pro Durchlauf erhöht dieser sich um +10/+11:
            spiel.PunktzahlAnzeigen()
            If Charakter = Charaktere.Yoda Then
                spiel.PunktzahlErhoehen(10)
            ElseIf Charakter = Charaktere.DarthVader Then
                spiel.PunktzahlErhoehen(11)
            End If

            'Zähler eins erhöhen, damit Herzen und Punkte spawnen
            zaehler += 1

            'Das Ganze wird wiederholt bis der/die Spieler kein(e) Leben mehr hat/haben:
        Loop Until spiel.SpielerIstGestorben()

        Console.Clear()

        GebeGameOverBilschirmAus(spiel.Punktzahl)

        'Score in Datei eintragen wenn Singleplayer:
        If Not Multiplayer Then
            SpeicherePunktzahl(Name, Charakter, spiel.Punktzahl, Schwierigkeit)
        End If

        'Hauptmenü wird wieder aufgerufen:
        Call OeffneHauptmenue()

    End Sub

    Sub OeffneHauptmenue()
        Dim gedrueckteTaste As Integer
        Dim ausgewaehlteZeile As Integer = 10

        Console.Clear()
        Console.ForegroundColor = ConsoleColor.DarkYellow
        Console.CursorVisible = False


        If Charakter = Charaktere.Yoda Then
            SpieleTonImHintergrundAb("Jedi")
        ElseIf Charakter = Charaktere.DarthVader Then
            SpieleTonImHintergrundAb("Sith")
        End If

        Console.SetCursorPosition(0, 7)
        GebeTextElementAus("Hauptmenue")

        ' Eingabe Schleife
        Do
            'Cursor anzeigen:
            Console.SetCursorPosition(50, ausgewaehlteZeile)
            Console.WriteLine(">")

            gedrueckteTaste = TastaturAbfrage
            If gedrueckteTaste = Tasten.Down And ausgewaehlteZeile < 16 Then
                ausgewaehlteZeile += 1
            ElseIf gedrueckteTaste = Tasten.Up And ausgewaehlteZeile < 10 Then
                ausgewaehlteZeile -= 1
            End If

            'Alte Cursor löschen:
            Console.SetCursorPosition(50, ausgewaehlteZeile - 1)
            Console.WriteLine(" ")
            Console.SetCursorPosition(50, ausgewaehlteZeile + 1)
            Console.WriteLine(" ")
        Loop Until gedrueckteTaste = Tasten.Enter

        Select Case ausgewaehlteZeile
            Case 10
                Console.Clear()
                Console.CursorVisible = False
                Call OeffneSchwierigkeitsMenue()
            Case 11
                Console.Clear()
                Console.CursorVisible = False
                Call OeffneAnleitung()
            Case 12
                Console.Clear()
                Console.CursorVisible = False
                Call OeffneScoreboard()
            Case 13
                Console.Clear()
                Console.CursorVisible = False
                Call OeffneCharakterAuswahl(False)
            Case 14
                Console.Clear()
                Console.CursorVisible = False
                Call OeffneNamenAendern()
            Case 15
                Console.Clear()
                Console.CursorVisible = False
                Console.WriteLine("Credits")
                Console.ReadKey()
                Call OeffneHauptmenue()
            Case 16
                Console.Clear()
                Console.CursorVisible = False
                End
        End Select
    End Sub

    Sub OeffneNamenAendern()
        Console.CursorVisible = True
        Console.Clear()
        Console.SetCursorPosition(0, 10)
        Console.WriteLine("                           Alles klar, gib einen neuen Namen ein und drücke 'ENTER':                                   ")
        Console.WriteLine("                                                  ->                                ")
        Console.SetCursorPosition(52, 11)
        Name = Console.ReadLine()
        Console.WriteLine("                                           Name erfolgreich geändert.")
        Console.CursorVisible = False
        Thread.Sleep(1500)
        Console.Clear()
        Call OeffneHauptmenue()
    End Sub

    Sub OeffneScoreboard()
        Dim eintraege As List(Of Tuple(Of String, String, String, String))
        Dim aktuelleZeile As Integer = 4 ' Ausgabe der Eintraege startet in Zeile 4
        Console.Clear()

        ' Die Eintraege kommen bereits sortiert aus der Funktion
        eintraege = LeseScoreboardEin()
        Console.SetCursorPosition(0, 1)
        Console.WriteLine("Scoreboard:")

        For Each eintrag In eintraege
            Console.SetCursorPosition(2, aktuelleZeile)
            ' Ausgabe der einzelnen Einträge
            ' -- Die Ausgabe ist noch nicht so schön - kannst du bspw. ändern indem man statt Tabs, den cursor setzt
            Console.WriteLine(eintrag.Item1 + vbTab + " - " + eintrag.Item2 + vbTab + vbTab + " als " + eintrag.Item3 + vbTab + eintrag.Item4)
            aktuelleZeile += 1
        Next

        Console.SetCursorPosition(0, aktuelleZeile + 2)
        Console.WriteLine("Drücke eine beliebige Taste um zurückzukehren...")

        Console.ReadKey()
        Console.Clear()
        Call OeffneHauptmenue()
    End Sub

    Sub OeffneSchwierigkeitsMenue()
        Dim gedrueckteTaste As Integer
        Dim ausgewaehlteZeile As Integer = 10

        Console.CursorVisible = False
        Console.SetCursorPosition(0, 8)
        GebeTextElementAus("Schwierigkeit")

        ' Vermeiden von Eingaben waehrend das Menue (noch) nicht angezeigt wird
        LeereTastaturBuffer()

        Do
            Console.SetCursorPosition(50, ausgewaehlteZeile)
            Console.WriteLine(">")
            gedrueckteTaste = TastaturAbfrage
            If gedrueckteTaste = Tasten.Down And ausgewaehlteZeile < 13 Then
                ausgewaehlteZeile += 1
            ElseIf gedrueckteTaste = Tasten.Up And ausgewaehlteZeile > 10 Then
                ausgewaehlteZeile -= 1
            End If

            Console.SetCursorPosition(50, ausgewaehlteZeile - 1)
            Console.WriteLine(" ")
            Console.SetCursorPosition(50, ausgewaehlteZeile + 1)
            Console.WriteLine(" ")
        Loop Until gedrueckteTaste = Tasten.Enter

        Select Case ausgewaehlteZeile
            Case 10
                Console.Clear()
                Schwierigkeit = Schwierigkeiten.Einfach
                Console.CursorVisible = False
                Call HauptSchleife()
            Case 11
                Console.Clear()
                Schwierigkeit = Schwierigkeiten.Mittel
                Console.CursorVisible = False
                Call HauptSchleife()
            Case 12
                Console.Clear()
                Schwierigkeit = Schwierigkeiten.Schwer
                Console.CursorVisible = False
                Call HauptSchleife()
            Case 13
                Console.Clear()
                Call OeffneHauptmenue()
        End Select
    End Sub


    Sub OeffneCharakterAuswahl(ByVal erstesMal As Boolean)
        Console.SetCursorPosition(0, 3)
        GebeTextElementAus("CharakterAuswahl")

        If erstesMal Then
            SpieleTonUndDanachSchleife("TieFighter", "BattleOfTheHeroes")
        Else
            SpieleTonImHintergrundAb("BattleOfTheHeroes")
        End If


        Console.CursorVisible = True

        ' Vermeiden von Eingaben waehrend das Menue (noch) nicht angezeigt wird
        LeereTastaturBuffer()

        Console.SetCursorPosition(53, 17)
        Select Case Console.ReadLine
            Case "1"
                Console.Clear()
                Charakter = Charaktere.Yoda
                WaehleYodaAus()
            Case "2"
                Console.Clear()
                Charakter = Charaktere.DarthVader
                WaehleDarthVaderAus()
            Case "3"
                Console.Clear()
                GebeYodaDatenAus()
            Case "4"
                Console.Clear()
                GebeDarthVaderDatenAus()
            Case Else
                Console.Clear()
                Console.SetCursorPosition(20, 12)
                Console.CursorVisible = False
                Console.WriteLine("      Ungültige Auswahl! Bitte gebe eine Zahl zwischen 1 und 4 ein.")
                Thread.Sleep(1500)
                Console.Clear()
                Call OeffneCharakterAuswahl(False)
        End Select
    End Sub

    Sub OeffneSpieleranzahlAuswahl()
        Console.CursorVisible = True

        ' Vermeiden von Eingaben waehrend das Menue (noch) nicht angezeigt wird
        LeereTastaturBuffer()

        Console.Clear()
        Console.SetCursorPosition(0, 10)
        ' -- Spieleranzahl
        GebeTextElementAus("Spieleranzahl")
        Console.SetCursorPosition(53, 17)

        Select Case Console.ReadLine()
            Case "1"
                Multiplayer = False
                Console.Clear()
            Case "2"
                Multiplayer = True
                Console.Clear()
            Case Else
                Console.Clear()
                Console.SetCursorPosition(20, 12)
                Console.WriteLine("      Ungültige Auswahl! Bitte gebe eine Zahl zwischen 1 und 2 ein.")
                Thread.Sleep(1500)
                Console.Clear()
                OeffneSpieleranzahlAuswahl()
        End Select

        Call OeffneHauptmenue()
    End Sub

    Sub OeffneAnleitung()
        Console.CursorVisible = False
        Console.SetCursorPosition(0, 2)
        GebeTextElementAus("Anleitung1")
        Console.ReadKey()
        Console.Clear()
        Console.SetCursorPosition(0, 2)
        GebeTextElementAus("Anleitung2")
        Console.Clear()
        Call OeffneHauptmenue()
    End Sub

    Sub WaehleDarthVaderAus()
        Console.ForegroundColor() = ConsoleColor.Red
        Console.SetCursorPosition(0, 2)
        GebeTextElementAus("DarthVader")
        SpieleTonImHintergrundAb("DarthVader")
        Thread.Sleep(1000)
        Console.Clear()
        OeffneSpieleranzahlAuswahl()
    End Sub

    Sub WaehleYodaAus()
        Console.ForegroundColor = ConsoleColor.Green
        Console.SetCursorPosition(0, 7)
        GebeTextElementAus("Yoda")
        SpieleTonImHintergrundAb("Yoda")
        Console.Clear()
        OeffneSpieleranzahlAuswahl()
    End Sub

    Sub GebeNamenEin()
        Console.SetCursorPosition(0, 10)
        Console.WriteLine("                       Herzlich Willkommen! Gib deinen Namen ein und schon geht es gleich los: ")
        Console.SetCursorPosition(52, 11)
        Console.WriteLine("-> ")
        Console.SetCursorPosition(55, 11)
        Name = Console.ReadLine()
    End Sub

    Sub LeereTastaturBuffer()
        While Console.KeyAvailable
            Console.ReadKey(True)
        End While
    End Sub


End Module
