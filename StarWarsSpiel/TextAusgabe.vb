Imports System.IO
Imports System.Threading

' Textblöcke
Module TextAusgabe
    Public Sub GebeTextElementAus(ByRef element As String)
        Dim projektVerzeichnis As String = Directory.GetParent(My.Application.Info.DirectoryPath).Parent.FullName
        Dim textElementVerzeichnis As String = Path.Combine(projektVerzeichnis, "resources/TextElemente/")

        Using reader As New StreamReader(textElementVerzeichnis + element + ".txt")
            While Not reader.EndOfStream
                Dim line As String = reader.ReadLine()
                Console.WriteLine(line)
            End While
        End Using
    End Sub

    Public Sub GebeLadebalkenAus()
        Console.CursorVisible = False
        GebeLadebalkenAsciiAus()
        Console.SetCursorPosition(0, 5)
        Console.Clear()
        Console.CursorVisible = True
        Console.Clear()
        Console.CursorVisible = False
        Console.SetCursorPosition(0, 7)
        ' -- Hinweis
        GebeTextElementAus("Hinweis")
        Console.ReadKey()
        Console.Clear()
    End Sub

    Public Sub GebeEroeffnungsTextAus()
        Console.SetCursorPosition(0, 4)
        GebeEroeffnungsTextAscii(300)
        Console.Clear()
    End Sub

    Public Sub GebeGameOverBilschirmAus(ByVal punktzahl As Integer)
        'Anzeige Game Over:
        Console.BackgroundColor = ConsoleColor.Black
        Console.SetCursorPosition(0, 10)
        GebeTextElementAus("GameOver")
        SpieleTonAbUndWarteAufEnde("Tod")
        Thread.Sleep(1000)
        Console.WriteLine("                               Drücke eine beliebige Taste um weiterzukommen")
        Console.ReadKey()

        'Score anzeigen
        Console.Clear()
        Console.SetCursorPosition(0, 12)
        Console.WriteLine("                                         Dein Score war: " & punktzahl)
        Console.WriteLine("")
        Console.WriteLine("                         Dücke eine beliebige Taste um ins Hauptmenü zurück zu kommen")
        Thread.Sleep(1000)
        Console.ReadKey()
        Console.Clear()
    End Sub


    Public Sub GebeDarthVaderDatenAus()
        Dim anzeigeGeschwindigkeit As Integer = 10

        Console.ForegroundColor = ConsoleColor.DarkRed
        Console.SetCursorPosition(0, 8)
        ' -- TieFighter
        GebeTextElementAus("TieFighter")
        Console.SetCursorPosition(2, 2)
        Console.WriteLine("Der TIE-Fighter:")
        Console.WriteLine()
        Console.WriteLine()
        SpieleTonImHintergrundAb("Tiping")

        Dim satz1 As String = "  Top-Speed: 1250km/h"
        GebeBuchstabenEinzelnAus(satz1, anzeigeGeschwindigkeit)
        Console.WriteLine()
        Console.WriteLine()

        Dim satz2 As String = "  Länge: 9 Meter"
        GebeBuchstabenEinzelnAus(satz2, anzeigeGeschwindigkeit)
        Console.WriteLine()
        Console.WriteLine()

        Dim satz3 As String = "  Stärke: Personalisierte Laserkanonen für "
        GebeBuchstabenEinzelnAus(satz3, anzeigeGeschwindigkeit)
        Console.WriteLine()

        Dim satz4 As String = "  Darth Vader"
        GebeBuchstabenEinzelnAus(satz4, anzeigeGeschwindigkeit)
        Console.WriteLine()
        Console.WriteLine()

        Dim satz5 As String = "  Schussweite: 6"
        GebeBuchstabenEinzelnAus(satz5, anzeigeGeschwindigkeit)
        Console.WriteLine()
        Console.WriteLine()

        Dim satz6 As String = "  Vorteile: Durch die Macht und die Laserkanonen kann der"
        GebeBuchstabenEinzelnAus(satz6, anzeigeGeschwindigkeit)
        Console.WriteLine()

        Dim satz7 As String = "  TIE-Fighter weiter schießen und ein wenig schneller punkten"
        GebeBuchstabenEinzelnAus(satz7, anzeigeGeschwindigkeit)
        Console.WriteLine()
        Console.WriteLine()
        Console.WriteLine()
        Console.WriteLine()

        Console.ForegroundColor = ConsoleColor.DarkYellow
        Console.WriteLine("  Drücke eine beliebige Taste um zur Auswahl zurückzukehren.")
        SpieleTonImHintergrundAb("Sith")
        Console.ReadKey()
        Console.Clear()
        Call OeffneCharakterAuswahl(False)
    End Sub

    Sub GebeYodaDatenAus()
        Dim anzeigeGeschwindigkeit As Integer = 10

        Console.ForegroundColor = ConsoleColor.Green
        Console.SetCursorPosition(0, 7)
        GebeTextElementAus("MilleniumFalcon")
        Console.SetCursorPosition(2, 2)
        Console.WriteLine("Der Millenium-Falcon:")
        Console.WriteLine()
        Console.WriteLine()
        SpieleTonImHintergrundAb("Tiping")

        Dim satz1 As String = "  Top-Speed: 9,31- fache Lichtgeschwindigkeit "
        GebeBuchstabenEinzelnAus(satz1, anzeigeGeschwindigkeit)
        Console.WriteLine()
        Console.WriteLine()

        Dim satz2 As String = "  Länge: 34,75 Meter"
        GebeBuchstabenEinzelnAus(satz2, anzeigeGeschwindigkeit)
        Console.WriteLine()
        Console.WriteLine()

        Dim satz3 As String = "  Stärke: 2 Militärische Schutzschilde und"
        GebeBuchstabenEinzelnAus(satz3, anzeigeGeschwindigkeit)
        Console.WriteLine()

        Dim satz4 As String = "  1 Antierschütterungsschild"
        GebeBuchstabenEinzelnAus(satz4, anzeigeGeschwindigkeit)
        Console.WriteLine()
        Console.WriteLine()

        Dim satz5 As String = "  Schussweite: 5"
        GebeBuchstabenEinzelnAus(satz5, anzeigeGeschwindigkeit)
        Console.WriteLine()
        Console.WriteLine()

        Dim satz6 As String = "  Vorteile: Durch die Starke Verteidigung kann der"
        GebeBuchstabenEinzelnAus(satz6, anzeigeGeschwindigkeit)
        Console.WriteLine()

        Dim satz7 As String = "  Millenium-Falke viel mehr aushalten als andere Raumschiffe"
        GebeBuchstabenEinzelnAus(satz7, anzeigeGeschwindigkeit)
        Console.WriteLine()
        Console.WriteLine()
        Console.WriteLine()
        Console.WriteLine()

        Console.ForegroundColor = ConsoleColor.DarkYellow
        Console.WriteLine("  Drücke eine beliebige Taste um zur Auswahl zurückzukehren.")
        SpieleTonImHintergrundAb("Jedi")

        Console.ReadKey()
        Console.Clear()
        Call OeffneCharakterAuswahl(False)
    End Sub

    Sub GebeBuchstabenEinzelnAus(ByRef eingabeString As String, ByVal anzeigeGeschwindigkeit As Integer)
        For Each buchstabe As Char In eingabeString
            Console.Write(buchstabe)
            Thread.Sleep(anzeigeGeschwindigkeit)
        Next
    End Sub

    Public Sub GebeVorspannAus()
        'Vorspann vor Auswahlmenü
        Console.BackgroundColor = ConsoleColor.Black
        Console.ForegroundColor = ConsoleColor.Cyan
        Console.CursorVisible = False
        Console.SetCursorPosition(0, 10)
        GebeTextElementAus("Intro1")
        Thread.Sleep(500)
        Console.Clear()

        SpieleTonImHintergrundAb("StarWars")

        Console.ForegroundColor = ConsoleColor.DarkYellow
        Console.BackgroundColor = ConsoleColor.Black

        Console.SetCursorPosition(0, 4)
        GebeTextElementAus("Intro2")
        Thread.Sleep(500)
        Console.Clear()


        Console.SetCursorPosition(0, 10)
        GebeTextElementAus("Intro3")
        Thread.Sleep(500)
        Console.Clear()

        Console.SetCursorPosition(0, 9)
        GebeTextElementAus("Intro4")
        Thread.Sleep(500)
        Console.Clear()
    End Sub

    Sub GebeLadebalkenAsciiAus()
        Console.WriteLine("                                                Spiel lädt...")
        Console.WriteLine("")
        Console.WriteLine("                                                     ()")
        Thread.Sleep(20)
        Console.WriteLine("                                                     []")
        Thread.Sleep(20)
        Console.WriteLine("                                                     ||")
        Thread.Sleep(10)
        Console.WriteLine("                                                     ||")
        Thread.Sleep(60)
        Console.WriteLine("                                                    .'`.")
        Thread.Sleep(200)
        Console.WriteLine("                                                    |  |")
        Thread.Sleep(30)
        Console.WriteLine("                                                    |  |")
        Thread.Sleep(200)
        Console.WriteLine("                                        |           |  |           |")
        Thread.Sleep(20)
        Console.WriteLine("                                        |           |  |           |")
        Thread.Sleep(300)
        Console.WriteLine("                                        |           |  |           |")
        Thread.Sleep(500)
        Console.WriteLine("                                        |       _  /    \  _       |")
        Thread.Sleep(20)
        Console.WriteLine("                                       |~|____.| |/      \| |.____|~|")
        Thread.Sleep(20)
        Console.WriteLine("                                       |                            |")
        Thread.Sleep(100)
        Console.WriteLine("                                       `-`-._                  _.-'-'")
        Thread.Sleep(20)
        Console.WriteLine("                                             `-.           _.-'     ")
        Thread.Sleep(50)
        Console.WriteLine("                                               ||\________/||")
        Thread.Sleep(20)
        Console.WriteLine("")
        Thread.Sleep(100)
        Console.WriteLine("                                               `'          `'")

        Thread.Sleep(200)
    End Sub

    Sub GebeEroeffnungsTextAscii(ByVal anzeigeGeschwindigkeit As Integer)
        Console.WriteLine("                     .     .            .        .          .       .                  .      .")
        Console.WriteLine("                   .     .                                                        .")
        Console.WriteLine("                               .   Düstere Zeiten sind angebrochen. Es ist   .        .     .")
        Thread.Sleep(anzeigeGeschwindigkeit)
        Console.WriteLine("                                  Krieg! Die dunkle Macht bedroht Friedrichs   .  .")
        Thread.Sleep(anzeigeGeschwindigkeit)
        Console.WriteLine("                      .       .  hafen mitsamt seinen Einwohnern. Die Bürger            .")
        Thread.Sleep(anzeigeGeschwindigkeit)
        Console.WriteLine("                 .        .     sind in Sorge und Fürchten sich zwecks der nahen                  .")
        Thread.Sleep(anzeigeGeschwindigkeit)
        Console.WriteLine("                    .          Bedrohung.   .      .")
        Thread.Sleep(anzeigeGeschwindigkeit)
        Console.WriteLine("                              Niemand anderes als der stärkste Krieger der Zeit, ")
        Thread.Sleep(anzeigeGeschwindigkeit)
        Console.WriteLine("                           . Darth Vader, möchte sein Imperium mit Hilfe seiner  .   .")
        Thread.Sleep(anzeigeGeschwindigkeit)
        Console.WriteLine("                    .       Raumschiffflotte ausbauen. Die Stadt Friedrichshafen ist        .  .")
        Thread.Sleep(anzeigeGeschwindigkeit)
        Console.WriteLine("                 .      .  dabei bei den Imperialisten extremst begehrt. Doch als eine   .")
        Thread.Sleep(anzeigeGeschwindigkeit)
        Console.WriteLine("                          der letzten verbleibenden, uneingenommenen Regionen wird sich             .")
        Thread.Sleep(anzeigeGeschwindigkeit)
        Console.WriteLine("                     .   Friedrichshafen bis zum Ende wehren.   .    .")
        Thread.Sleep(anzeigeGeschwindigkeit)
        Console.WriteLine("                 .      Dabei bekommen die tapferen Einwohner Hilfe von niemand anderem als     .")
        Thread.Sleep(anzeigeGeschwindigkeit)
        Console.WriteLine("                     . dem Jedi-Großmeister Yoda selbst.")
        Thread.Sleep(anzeigeGeschwindigkeit)
        Console.WriteLine("                      Er ist die große Hoffnung und Unterstützung, die es benötigt die dunkle            .")
        Thread.Sleep(anzeigeGeschwindigkeit)
        Console.WriteLine("                     Seite aufzuhalten.  .")
        Thread.Sleep(anzeigeGeschwindigkeit)
        Console.WriteLine("")
        Console.WriteLine("                  . Werden es die Einwohner schaffen Friedrichshafen erfolgreich zu verteidigen")
        Thread.Sleep(anzeigeGeschwindigkeit)
        Console.WriteLine("                   oder wird schon bald das Böse über die Stadt herrschen.")
        Thread.Sleep(anzeigeGeschwindigkeit)
        Console.WriteLine("                  Nur DU kannst es herausfinden! Denn die Macht ist stark in dir! ALSO LOS GEHTS!             .         .")
        Thread.Sleep(anzeigeGeschwindigkeit)
        Console.WriteLine("                 .        .          .    .    .            .            .                   .")
        Thread.Sleep(anzeigeGeschwindigkeit)
        Thread.Sleep(4000)
    End Sub
End Module
