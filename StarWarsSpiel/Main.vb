Imports System.Data.SqlTypes
Imports System.Net.WebRequestMethods
Imports System.Threading
Imports System.Xml.Schema
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.FileIO
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices
Imports System.CodeDom
Imports System.Configuration
Imports System.Diagnostics.Eventing.Reader


Module Main
    ' Konstanten zur Darstellung von verschiedenen Tastencodes
    ' -- Hier ein Enum verwenden
    Const NO_KEY = 0
    Const CURSOR_LEFT = 1
    Const CURSOR_RIGHT = 2
    Const UNKNOWN_KEY = 99
    Const KEY_SPACE = 3
    Const CURSOR_UP = 7
    Const CURSOR_DOWN = 8
    Const KEY_P = 9
    Const KEY_W = 10
    Const KEY_A = 11
    Const KEY_S = 12
    Const KEY_D = 13
    Const KEY_Y = 14
    Const KEY_V = 15
    Const KEY_ENTER = 23


    ' Konstanten für das Spielfeld und die Hindernisse
    ' -- Hier auch ein Enum
    Const SPALTE_MAX = 79 ' Maximale Anzahl der Spalten im Spielfeld
    Const SPALTE_MIN = 0 ' Minimale Anzahl der Spalten im Spielfeld
    Const ZEILE_MAX = 24 ' Maximale Anzahl der Zeilen im Spielfeld
    Const A_MIN = 2 'Mindestanzahl Hindernisblocks pro Zeile
    Const GESCHW_SPIELFIGUR = 5 ' Konstante für die Geschwindigkeit der Spielfigur (Anzahl der Spalten pro Takt)

    'Globale Variablen für Schwierigkeit etc.:
    ' -- Globale Variablen sind tendenziell unschön - bspw. eine Klasse alternativ?
    Dim G_min As Integer  'Mindestgröße eines Hindernisblocks 
    Dim G_max As Integer 'Maximalgröße eines Hindernisblocks 
    Dim Einstellung As String 'Variable zur Speicherung der Schwierigkeitsstufe
    Dim Charakter As String 'Auswahl Spielfigur 
    Dim Score As Integer      'Highscore
    Dim Namen As String         'Namen des Spielers
    Dim CharaAuswahl As Integer = 0 ' Flag, um zu verhindern, dass ein Sound zweimal abgespielt wird
    Dim Multiplayer As Integer = 0 ' Flag für den Multiplayer-Modus


    ' Funktion zur Abfrage der Tastatureingabe
    Function Tastatur_Abfrage() As Integer
        Dim cki As New ConsoleKeyInfo()
        If Console.KeyAvailable = False Then
            Return NO_KEY
            ' -- Das Else ist unnötig - das If returned sowieso
        Else
            cki = Console.ReadKey(True)
            ' -- Switch Case verwenden
            If cki.Key = ConsoleKey.LeftArrow Then
                Return CURSOR_LEFT
            ElseIf cki.Key = ConsoleKey.RightArrow Then
                Return CURSOR_RIGHT
            ElseIf cki.Key = ConsoleKey.Spacebar Then
                Return KEY_SPACE
            ElseIf cki.Key = ConsoleKey.UpArrow Then
                Return CURSOR_UP
            ElseIf cki.Key = ConsoleKey.DownArrow Then
                Return CURSOR_DOWN
            ElseIf cki.Key = ConsoleKey.P Then
                Return KEY_P
            ElseIf cki.Key = ConsoleKey.W Then
                Return KEY_W
            ElseIf cki.Key = ConsoleKey.A Then
                Return KEY_A
            ElseIf cki.Key = ConsoleKey.S Then
                Return KEY_S
            ElseIf cki.Key = ConsoleKey.D Then
                Return KEY_D
            ElseIf cki.Key = ConsoleKey.Y Then
                Return KEY_Y
            ElseIf cki.Key = ConsoleKey.V Then
                Return KEY_V
            ElseIf cki.Key = ConsoleKey.Enter Then
                Return KEY_ENTER
            Else
                Return UNKNOWN_KEY
            End If
        End If
    End Function

    'Prozedur zur Erzeugung einer neuen Zeile von Hindernissen
    Sub ErzeugeZeile(ByRef Zeile() As Char, ByVal a_max As Integer)
        ' -- Die Variablen Namen sind nicht aussagekräffitg
        Dim x As Single                 'Zufallszahl
        Dim A As Integer                'Anzahl der Hindernisblocks
        Dim i As Integer                'Laufvariable 
        Dim G As Integer                'Größe des Hindernisblocks
        Dim P As Integer                'Startposition des Hindernisblocks
        Dim j As Integer                'Laufvariable

        'Zeilenvektor mit Leerzeichen vorbelegen:
        For i = SPALTE_MIN To SPALTE_MAX
            Zeile(i) = " "
        Next

        'Festlegung der Hindernisgrößen basierend auf der Schwierigkeitseinstellung
        ' -- Hier auch switch case
        ' -- Die Schwierigkeit hier als String abzufragen ist nicht schön
        ' -- "Einstellung" ist kein guter Name, da das alles heißen könnte
        If Einstellung = "einfach" Then
            G_min = 1
            G_max = 1
        ElseIf Einstellung = "mittel" Then
            G_min = 5
            G_max = 8
        ElseIf Einstellung = "schwer" Then
            G_min = 8
            G_max = 14
        End If

        'Anzahl A der Hindernisblocks zufällig festlegen:
        ' -- Hier sieht man die Problematik der unaussagekräftigen Variablennamen
        x = VBMath.Rnd
        A = (a_max - A_MIN) * x + A_MIN

        'Für jeden der A Hindernisblocks:
        For i = 1 To A

            'Größe G des Hindernisblocks zufällig festlegen:
            x = VBMath.Rnd
            G = (G_max - G_min) * x + G_min

            'Startposition des Hindernisblocks zufällig festlegen:
            x = VBMath.Rnd
            P = (SPALTE_MAX - SPALTE_MIN) * x + SPALTE_MIN

            'Für jedes der G Einzelhindernisse, Laufvariable:
            For j = 1 To G
                'Prüfen, ob das Hindernis innerhalb des Vektors liegt
                If P + j - 1 <= SPALTE_MAX Then
                    'Hindernis an der Position P + j - 1 in den Vektor schreiben
                    Zeile(P + j - 1) = "■"
                End If
            Next
        Next
    End Sub

    'Prozedur zur Erzeugung eines Herzens in der Zeile
    Sub ErzeugeHerz(ByRef Zeile() As Char)
        ' -- Unaussagekräftige Variablennamen
        Dim P As Integer                'Startposition des Herz
        Dim x As Single                 'Zufallszahl
        'Startposition des Herzen zufällig festlegen:
        x = VBMath.Rnd
        P = (SPALTE_MAX - SPALTE_MIN) * x + SPALTE_MIN
        For j = 1 To 1
            'Prüfen, ob das Herz innerhalb des Vektors liegt
            If P + j - 1 <= SPALTE_MAX Then
                'Herz an der Position P + j - 1 in den Vektor schreiben
                Zeile(P + j - 1) = "♥"
            End If
        Next
    End Sub

    'Prozedur zur Erzeugung eines +5 Punkt in der Zeile
    Sub Erzeuge5Punkte(ByRef Zeile() As Char)
        Dim P As Integer                'Startposition der 5
        Dim x As Single                 'Zufallszahl
        'Startposition der 5 zufällig festlegen:
        x = VBMath.Rnd
        P = (SPALTE_MAX - SPALTE_MIN) * x + SPALTE_MIN
        For j = 1 To 1
            'Prüfen, ob die 5 innerhalb des Vektors liegt
            If P + j - 1 <= SPALTE_MAX Then
                '5 an der Position P + j - 1 in den Vektor schreiben
                Zeile(P + j - 1) = "5"
            End If
        Next
    End Sub

    'Prozedur zur Erzeugung *2 in der Zeile
    Sub Erzeuge2Punkte(ByRef Zeile() As Char)
        Dim P As Integer                'Startposition der 2
        Dim x As Single                 'Zufallszahl
        'Startposition der 2 zufällig festlegen:
        x = VBMath.Rnd
        P = (SPALTE_MAX - SPALTE_MIN) * x + SPALTE_MIN
        For j = 1 To 1
            'Prüfen, ob das 2 innerhalb des Vektors liegt
            If P + j - 1 <= SPALTE_MAX Then
                '2 an der Position P + j - 1 in den Vektor schreiben
                Zeile(P + j - 1) = "2"
            End If
        Next
    End Sub

    'Prozedur zur Erzeugung eines Schussgadgets in der Zeile
    Sub ErzeugeSchussDrop(ByRef Zeile() As Char)
        Dim P As Integer                'Startposition des Schusssymblos
        Dim x As Single                 'Zufallszahl
        'Startposition des Schusssymblos zufällig festlegen:
        x = VBMath.Rnd
        P = (SPALTE_MAX - SPALTE_MIN) * x + SPALTE_MIN
        For j = 1 To 1
            'Prüfen, ob das Schusssymblos innerhalb des Vektors liegt
            If P + j - 1 <= SPALTE_MAX Then
                'Schusssymblos an der Position P + j - 1 in den Vektor schreiben
                Zeile(P + j - 1) = "|"
            End If
        Next
    End Sub
    ' -- Die Funktionen Erzeuge2Punkte und Erzeuge5Punkte und ErzeugeSchussDrop sind quasi gleich - es wäre besser die
    ' -- zusammenzufassen und statdessen die Zahl als Parameter zu übergeben


    'Hauptprozedur für den Spielablauf
    Sub Spielablauf()
        ' -- Wenn euer Prof das so gemacht hat, dann würde ich die Variablen auch weiterhin oben deklarieren
        ' -- Jedoch würde ich das bei Laufvariablen trotzdem nicht machen - da ist es wirklich sehr sehr unschön
        ' -- Konsistent bei variablen Namen! Bspw. sollte Zähler nicht groß sein
        Dim leben1 As Integer               'Anzahl der verbleibenden Leben für Spieler 1
        Dim leben2 As Integer               'Anzahl der verbleibenden Leben für Spieler 2 (Multiplayer)
        Dim s, z As Integer                 'Laufvariablen für Spalte und Zeile 
        Dim spielfeld(ZEILE_MAX, SPALTE_MAX) As Char       'Spielfeld-Matrix
        Dim Zeile(SPALTE_MAX) As Char       'Zeilenvektor der Hindernisse
        Dim taste As Integer                'gedrückte Taste 
        Dim spielfigur_1s As Integer        'Spalte der Spielfigur 1
        Dim spielfigur_1z As Integer        'Zeile der Spielfigur 1
        Dim spielfigur_2s As Integer        'Spalte der Spielfigur 2 (Multiplayer)
        Dim spielfigur_2z As Integer        'Zeile der Spielfigur 2 (Multiplayer)
        Dim i, u, k As Integer              'Laufvariable
        Dim wartezeit As Single             'Wartezeit
        Dim a_max As Integer                'Max Anzahl der Hindernisse
        Dim a_max_zähler As Integer         'Zähler für die Erhöhung der hindernisse
        Dim Zaehler As Integer              'Zählvariable, damit die die Prozeduren von 'Erzeuge...' unterschiedlich häufig gecallt werden können
        Dim schusszähler As Integer         'Zähler für Schuss

        'Startwerte setzen:
        'Wenn Charakter Yoda 5 Leben
        ' -- Die Charactere hier als Strings zu Hardcoden ist unschön
        If Charakter = "Yoda" Then
            leben1 = 5
            leben2 = 5
            'Wenn Charakter Darth Vader 5 Leben
        ElseIf Charakter = "Darth Vader" Then
            leben1 = 3
            leben2 = 3
        End If

        'Startpositionen der Spielfiguren festlegen
        'Startposition Multiplayer
        If Multiplayer = 0 Then
            spielfigur_1s = SPALTE_MAX / 2
            spielfigur_1z = ZEILE_MAX
            'Startpositionen Singlepalyer
        ElseIf Multiplayer = 1 Then
            spielfigur_1s = SPALTE_MAX / 2
            spielfigur_1z = ZEILE_MAX
            spielfigur_2s = SPALTE_MAX / 4
            spielfigur_2z = ZEILE_MAX
        End If

        ' -- Die Variablen hier zu initialisieren ist unschön, lieber direkt oben
        wartezeit = 500                     'Initiale Wartezeit
        a_max = 5                           'Initiale maximale Anzahl der Hindernisse
        Score = -200                        'Initialer Score damit wenn Hindernisse unten bei Spielfigur sind Score = 0 ist
        Zaehler = 0                         'Zähler, der pro Runde erhöht wird um die Gadgets alle paar Zeilen erscheinen zu lassen
        schusszähler = 4                    'Zählt die verfügbaren Schüsse des Spielers


        ' -- Dateinamen sind auch inkonsistent was Groß- und Kleinschreibung angeht
        My.Computer.Audio.Play("Imperial March.wav", AudioPlayMode.BackgroundLoop)          'Hintergrundmusik

        'Hauptschleife des Spiels:
        Do
            'Neue Hinderniszeile erzeugen:
            Call ErzeugeZeile(Zeile, a_max)


            '-- Wie wäre es hier die Gadgets etwas mehr random zu erzugen?
            'Gadgets:

            'Neue Herzen alle drei Zeilen erzeugen:
            If Zaehler Mod 3 = 0 Then
                Call ErzeugeHerz(Zeile)
            End If

            '+5 Punkte alle 2 Zeilen erzeugen:
            If Zaehler Mod 2 = 0 Then
                Call Erzeuge5Punkte(Zeile)
            End If

            '*2 Punkte alle 15 Zeilen erzeugen:
            If Zaehler Mod 15 = 0 Then
                Call Erzeuge2Punkte(Zeile)
            End If

            '+Schuss alle 2 Zeilen erzeugen (nur Singleplayer):
            If Zaehler Mod 2 = 0 Then
                If Multiplayer = 0 Then
                    Call ErzeugeSchussDrop(Zeile)
                End If
            End If

            'Hindernisse um eine Zeile nach unten verschieben:
            For z = ZEILE_MAX To 1 Step -1
                For s = 0 To SPALTE_MAX
                    spielfeld(z, s) = spielfeld(z - 1, s)
                Next
            Next

            'Neue Zeile in die erste Zeile des Spielfelds übertragen:
            For s = 0 To SPALTE_MAX
                spielfeld(0, s) = Zeile(s)
            Next

            'Spielfeld auf der Konsole ausgeben:
            Console.SetCursorPosition(0, 0)
            'Farben der Gadgets ändern:
            ' -- Das Hardcoding der Symbole ist unschön
            For z = 0 To 23
                For s = 0 To SPALTE_MAX
                    ' -- Hier switch case
                    If spielfeld(z, s) = "|" Then
                        Console.ForegroundColor = ConsoleColor.DarkYellow
                    ElseIf spielfeld(z, s) = "2" Then
                        Console.ForegroundColor = ConsoleColor.White
                    ElseIf spielfeld(z, s) = "5" Then
                        Console.ForegroundColor = ConsoleColor.Cyan
                    ElseIf spielfeld(z, s) = "♥" Then
                        If Charakter = "Yoda" Then
                            Console.ForegroundColor = ConsoleColor.DarkGreen
                        ElseIf Charakter = "Darth Vader" Then
                            Console.ForegroundColor = ConsoleColor.DarkRed
                        End If
                    Else
                        'Farbe je nach Charakter anpassen:
                        If Charakter = "Yoda" Then
                            Console.ForegroundColor = ConsoleColor.DarkRed
                        ElseIf Charakter = "Darth Vader" Then
                            Console.ForegroundColor = ConsoleColor.Green
                        End If
                    End If
                    Console.Write(spielfeld(z, s))
                Next
                Console.WriteLine()
            Next

            'Zählschleife für schnellere Bewegungen:
            For i = 1 To GESCHW_SPIELFIGUR

                'Alte Spielfigur1 löschen:
                Console.SetCursorPosition(spielfigur_1s, spielfigur_1z)
                Console.Write(" ")

                'Alte Spielfigur2 löschen (Multiplayer):
                Console.SetCursorPosition(spielfigur_2s, spielfigur_2z)
                Console.Write(" ")

                'Tastatur abfragen:
                taste = Tastatur_Abfrage()

                'Position der Spielfigur berechnen:
                ' -- Hier switch case
                If taste = CURSOR_LEFT Then
                    spielfigur_1s = spielfigur_1s - 1
                    If spielfigur_1s < 0 Then spielfigur_1s = 0
                End If

                If taste = CURSOR_RIGHT Then
                    spielfigur_1s = spielfigur_1s + 1
                    If spielfigur_1s > SPALTE_MAX Then spielfigur_1s = SPALTE_MAX
                End If

                If taste = CURSOR_UP Then
                    spielfigur_1z = spielfigur_1z - 1
                    If spielfigur_1z < 20 Then spielfigur_1z = 20
                End If

                If taste = CURSOR_DOWN Then
                    spielfigur_1z = spielfigur_1z + 1
                    If spielfigur_1z > ZEILE_MAX Then spielfigur_1z = ZEILE_MAX
                End If

                If taste = KEY_W Then
                    spielfigur_2z = spielfigur_2z - 1
                    If spielfigur_2z < 20 Then spielfigur_2z = 20
                End If

                If taste = KEY_A Then
                    spielfigur_2s = spielfigur_2s - 1
                    If spielfigur_2s < 0 Then spielfigur_2s = 0
                End If

                If taste = KEY_S Then
                    spielfigur_2z = spielfigur_2z + 1
                    If spielfigur_2z > ZEILE_MAX Then spielfigur_2z = ZEILE_MAX
                End If

                If taste = KEY_D Then
                    spielfigur_2s = spielfigur_2s + 1
                    If spielfigur_2s > SPALTE_MAX Then spielfigur_2s = SPALTE_MAX
                End If

                ' -- TODO: Angucken
                'Spielfigur auf der Konsole ausgeben (Singleplayer):
                If Multiplayer = 0 Then
                    Console.SetCursorPosition(spielfigur_1s, spielfigur_1z)
                    'Wenn Yoda der Charakter ist:
                    If Charakter = "Yoda" Then
                        Console.ForegroundColor = ConsoleColor.Green
                        Console.Write("=")
                        'Wenn Darth Vader der Charakter ist:
                    ElseIf Charakter = "Darth Vader" Then
                        Console.ForegroundColor = ConsoleColor.DarkRed
                        Console.Write("!")
                    End If

                    'Spielfigur Multiplayer ausgeben:
                ElseIf Multiplayer = 1 Then
                    'Wenn Yoda der Charakter ist:
                    If Charakter = "Yoda" Then
                        Console.ForegroundColor = ConsoleColor.Green
                        Console.SetCursorPosition(spielfigur_1s, spielfigur_1z)
                        Console.Write("1")
                        Console.SetCursorPosition(spielfigur_2s, spielfigur_2z)
                        Console.Write("2")
                        'Wenn Darth Vader der Charakter ist:
                    ElseIf Charakter = "Darth Vader" Then
                        Console.ForegroundColor = ConsoleColor.DarkRed
                        Console.SetCursorPosition(spielfigur_1s, spielfigur_1z)
                        Console.Write("1")
                        Console.SetCursorPosition(spielfigur_2s, spielfigur_2z)
                        Console.Write("2")
                    End If
                End If

                'Schuss wird ausgegeben:
                'Sofern der Spieler ein '|' berührt hat (Kollisionsprüfung für bessere Übersicht weiter oben als die anderen):
                If spielfeld(spielfigur_1z - 1, spielfigur_1s) = "|" Then
                    schusszähler = schusszähler + 1
                    spielfeld(spielfigur_1z - 1, spielfigur_1s) = " "
                End If
                'Kann der Spieler einen Schuss abfeuern:
                ' -- Sehr tief geschachtelt ab hier - weiß noch nicht ob mans einfach ändern kann
                If schusszähler >= 1 Then
                    If taste = KEY_SPACE Then
                        Console.ForegroundColor = ConsoleColor.Blue
                        If Charakter = "Yoda" Then
                            For k = 1 To 5
                                Console.SetCursorPosition(spielfigur_1s, spielfigur_1z - k)
                                Console.WriteLine("|")
                                Thread.Sleep(100)
                            Next
                        ElseIf Charakter = "Darth Vader" Then
                            For k = 1 To 6
                                Console.SetCursorPosition(spielfigur_1s, spielfigur_1z - k)
                                Console.WriteLine("|")
                                Thread.Sleep(100)
                            Next
                        End If
                        My.Computer.Audio.Play("Blaster.wav", AudioPlayMode.WaitToComplete)
                        My.Computer.Audio.Play("Imperial March.wav", AudioPlayMode.BackgroundLoop)
                        'Schüsse nach abfeuern um 1 reduzieren:
                        schusszähler = schusszähler - 1
                        'Kollisionsprüfung nach russischer Methode:
                        ' -- Switch Case
                        If spielfeld(spielfigur_1z - 1, spielfigur_1s) = "■" Then
                            spielfeld(spielfigur_1z - 1, spielfigur_1s) = " "
                        End If
                        If spielfeld(spielfigur_1z - 2, spielfigur_1s) = "■" Then
                            spielfeld(spielfigur_1z - 2, spielfigur_1s) = " "
                        End If
                        If spielfeld(spielfigur_1z - 3, spielfigur_1s) = "■" Then
                            spielfeld(spielfigur_1z - 3, spielfigur_1s) = " "
                        End If
                        If spielfeld(spielfigur_1z - 4, spielfigur_1s) = "■" Then
                            spielfeld(spielfigur_1z - 4, spielfigur_1s) = " "
                        End If
                        If spielfeld(spielfigur_1z - 5, spielfigur_1s) = "■" Then
                            spielfeld(spielfigur_1z - 5, spielfigur_1s) = " "
                        End If
                        'Nur Bei Darth Vader wird das 6. Feld über der Spielfigur gelöscht:
                        If Charakter = "Darth Vader" Then
                            If spielfeld(spielfigur_1z - 6, spielfigur_1s) = "■" Then
                                spielfeld(spielfigur_1z - 6, spielfigur_1s) = " "
                            End If
                        End If
                    End If
                End If

                'Da nur bei Singleplayer schuss existiert, wird auch nur die Anzahl der Schüsse bei Singleplayer angezeigt:
                If Multiplayer = 0 Then
                    Console.ForegroundColor = ConsoleColor.Blue
                    Console.SetCursorPosition(26, 26)
                    Console.WriteLine("Verbleibende Schüsse: " & schusszähler)
                End If

                'Pausenfunktion 
                If taste = KEY_P Then
                    Console.Clear()
                    My.Computer.Audio.Play("Wartemusik.wav", AudioPlayMode.BackgroundLoop)
                    Console.ForegroundColor = ConsoleColor.DarkYellow
                    Console.SetCursorPosition(0, 10)
                    ' -- Text in Textdateien auslagern - Parhmi war der Meinung, dass das bei eurem Prof nicht gut ankommt im Code
                    Console.WriteLine("                                         Das Spiel ist pausiert")
                    Console.WriteLine("                                      P drücken um weiterzuspielen")
                    Do
                        taste = Tastatur_Abfrage()
                    Loop Until taste = KEY_P
                    Console.Clear()
                    If Charakter = "Yoda" Then
                        ' -- Imperial March bei beiden?
                        My.Computer.Audio.Play("Imperial March.wav", AudioPlayMode.BackgroundLoop)
                    ElseIf Charakter = "Darth Vader" Then
                        My.Computer.Audio.Play("Imperial March.wav", AudioPlayMode.BackgroundLoop)
                    End If
                End If

                'Kollisionprüfung 1. Spieler (auch Singleplayer):
                ' -- Kollisionsprüfungen ist 2mal das gleiche, in Funktion
                ' -- Switch Case
                If spielfeld(spielfigur_1z, spielfigur_1s) = "■" Then
                    My.Computer.Audio.Play("Tod.wav", AudioPlayMode.WaitToComplete)
                    My.Computer.Audio.Play("Imperial March.wav", AudioPlayMode.BackgroundLoop)
                    'Leben werden um 1 reduziert:
                    leben1 = leben1 - 1
                    'Hindernis wird gelöscht:
                    spielfeld(spielfigur_1z, spielfigur_1s) = " "
                End If

                'Kollisionprüfung 2. Spieler ( nur Multiplayer):
                If Multiplayer = 1 Then
                    If spielfeld(spielfigur_2z, spielfigur_2s) = "■" Then
                        My.Computer.Audio.Play("Tod.wav", AudioPlayMode.WaitToComplete)
                        My.Computer.Audio.Play("Imperial March.wav", AudioPlayMode.BackgroundLoop)
                        'Leben werden um 1 reduziert: 
                        leben2 = leben2 - 1
                        'Hindernis wird gelöscht:
                        spielfeld(spielfigur_2z, spielfigur_2s) = " "
                    End If
                End If

                'Kollisionprüfung Herz 1. Spieler (auch Singleplayer):
                If spielfeld(spielfigur_1z, spielfigur_1s) = "♥" Then
                    leben1 = leben1 + 1
                    Thread.Sleep(200)
                    spielfeld(spielfigur_1z, spielfigur_1s) = " "
                End If

                'Kollisionprüfung Herz 2. Spieler (nur Multiplayer):
                If Multiplayer = 1 Then
                    If spielfeld(spielfigur_2z, spielfigur_2s) = "♥" Then
                        leben2 = leben2 + 1
                        Thread.Sleep(200)
                        spielfeld(spielfigur_2z, spielfigur_2s) = " "
                    End If
                End If

                'Kollisionprüfung 5 Punkte 1. Spieler:
                If spielfeld(spielfigur_1z, spielfigur_1s) = "5" Then
                    Score = Score + 5
                    Thread.Sleep(200)
                    spielfeld(spielfigur_1z, spielfigur_1s) = " "
                End If


                'Kollisionprüfung 5 Punkte 2. Spieler (Multiplayer):
                If Multiplayer = 1 Then
                    If spielfeld(spielfigur_2z, spielfigur_2s) = "5" Then
                        Score = Score + 5
                        Thread.Sleep(200)
                        spielfeld(spielfigur_2z, spielfigur_2s) = " "
                    End If
                End If

                'Kollisionprüfung *2 Punkte 1. Spieler (auch Singleplayer):
                If spielfeld(spielfigur_1z, spielfigur_1s) = "2" Then
                    Score = Score * 2
                    Thread.Sleep(200)
                    spielfeld(spielfigur_1z, spielfigur_1s) = " "
                End If

                'Kollisionprüfung *2 Punkte 2. Spieler (nur Multiplayer):
                If Multiplayer = 1 Then
                    If spielfeld(spielfigur_2z, spielfigur_2s) = "2" Then
                        Score = Score * 2
                        Thread.Sleep(200)
                        spielfeld(spielfigur_2z, spielfigur_2s) = " "
                    End If
                End If

                'Kollisionprüfung *2 Punkte 1. Spieler (auch Singleplayer):
                If Multiplayer = 1 Then
                    If spielfigur_2z = "1" Then
                        spielfigur_2z = spielfigur_2z - 1
                    End If
                    If spielfeld(spielfigur_1z, spielfigur_1s) = "2" Then
                        spielfeld(spielfigur_1z, spielfigur_1s) = spielfeld(spielfigur_1z - 1, spielfigur_1s - 1)
                    End If
                End If

                'Anzahl der Leben ausgeben für Spieler 1:
                'Farbe anpassen:
                If Charakter = "Yoda" Then
                    Console.ForegroundColor = ConsoleColor.Green
                ElseIf Charakter = "Darth Vader" Then
                    Console.ForegroundColor = ConsoleColor.DarkRed
                End If
                Console.SetCursorPosition(50, 25)
                Console.Write("Leben: ")
                'Für die Anzahl der Leben:
                For u = 1 To leben1
                    'Wird das Herz Symbol angezeigt:
                    Console.Write("♥ ")
                Next
                Console.Write("  ")

                'Anzahl der Leben ausgeben für Spieler 2 (Multiplayer):
                If Multiplayer = 1 Then
                    Console.SetCursorPosition(0, 25)
                    Console.Write("Leben: ")
                    For u = 1 To leben2
                        Console.Write("♥ ")
                    Next
                    Console.Write("  ")
                End If

                'Warten:
                Thread.Sleep(wartezeit / GESCHW_SPIELFIGUR)
            Next

            'Tastaturpuffer löschen:
            Do
                taste = Tastatur_Abfrage()
            Loop Until taste = NO_KEY

            'Geschwindigkeit erhöhen:
            wartezeit = wartezeit * 0.99

            'Hindernisdichte erhöhen:
            a_max_zähler = a_max_zähler + 1
            If a_max_zähler = 20 Then
                a_max_zähler = 0
                a_max = a_max + 1
            End If

            'Score wird angezeigt:
            'Pro Durchlauf erhöht dieser sich um +10:
            Score = Score + 10
            Console.SetCursorPosition(33, 25)
            Console.WriteLine("Score: ")
            If Charakter = "Yoda" Then
                If Score < 0 Then
                    Console.SetCursorPosition(40, 25)
                    'Da der Score zu Beginn negativ definiert ist, wird bis zur 0 '0' angezeigt:
                    Console.Write("0")
                ElseIf Score > 0 Then
                    Console.SetCursorPosition(40, 25)
                    'Sofern Score positiv wird der richtige Score angezeigt:
                    Console.Write(Score)
                End If
            ElseIf Charakter = "Darth Vader" Then
                If Score < 0 Then
                    Console.SetCursorPosition(40, 25)
                    Console.Write("0")
                ElseIf Score > 0 Then
                    'Darth Vader bekommt Charakterbedingt +1 mehr als Score pro Durchlauf:
                    Score = Score + 1
                    Console.SetCursorPosition(40, 25)
                    Console.Write(Score)
                End If
            End If

            'Zähler eins erhöhen, damit Herzen und Punkte spawnen
            Zaehler = Zaehler + 1

            'Das Ganze wird wiederholt bis der Spieler sein leben verloren hat oder Spieler 1 oder Spieler 2 im Multiplayer alle Leben verloren hat:
        Loop Until leben1 <= 0 Or leben2 <= 0

        Console.Clear()

        'Anzeige Game Over:
        Console.BackgroundColor = ConsoleColor.Black
        Console.SetCursorPosition(0, 10)
        Console.WriteLine("                          _  _____                         ____                 _")
        Console.WriteLine("                         | |/ ____|                       / __ \               | |")
        Console.WriteLine("                         | | |  __  __ _ _ __ ___   ___  | |  | |_   _____ _ __| |")
        Console.WriteLine("                         | | | |_ |/ _` | '_ ` _ \ / _ \ | |  | \ \ / / _ \ '__| |")
        Console.WriteLine("                         |_| |__| | (_| | | | | | |  __/ | |__| |\ V /  __/ |  |_|")
        Console.WriteLine("                         (_)\_____|\__,_|_| |_| |_|\___|  \____/  \_/ \___|_|  (_)")
        Console.WriteLine("")
        My.Computer.Audio.Play("Tod.wav", AudioPlayMode.WaitToComplete)
        Thread.Sleep(1000)
        Console.WriteLine("                               Drücke eine beliebige Taste um weiterzukommen")
        Console.ReadKey()

        'Score anzeigen
        Console.Clear()
        Console.SetCursorPosition(0, 12)
        Console.WriteLine("                                         Dein Score war: " & Score)
        Console.WriteLine("")
        Console.WriteLine("                         Dücke eine beliebige Taste um ins Hauptmenü zurück zu kommen")
        Thread.Sleep(1000)
        Console.ReadKey()
        Console.Clear()

        'Score in Datei eintragen wenn Singleplayer:
        If Multiplayer = 0 Then
            My.Computer.FileSystem.WriteAllText("highscore.txt", Score & "," & Namen & "," & Charakter & vbCrLf, True)
        End If

        'Hauptmenü wird wieder aufgerufen:
        Call menu()

    End Sub

    Sub menu()
        'Hauptmenü
        'Dim choice1 As String
        Dim taste As Integer
        Dim zeile As Integer = 10
        Console.Clear()
        Console.ForegroundColor = ConsoleColor.DarkYellow
        Console.CursorVisible = False
        'taste = Tastatur_Abfrage()
        'Menü anzeigen
        If Charakter = "Yoda" Then
            My.Computer.Audio.Play("Jedi.wav", AudioPlayMode.BackgroundLoop)
        ElseIf Charakter = "Darth Vader" Then
            My.Computer.Audio.Play("Sith.wav", AudioPlayMode.BackgroundLoop)
        End If
        Console.SetCursorPosition(0, 7)
        Console.WriteLine("            Wähle eine Option aus indem du mit den Cursor-Tasten den Cursor nach oben oder unten bewegst.")
        Console.WriteLine("                        Sobald du dich für eine Option Entschieden hast drücke 'Enter'")
        Console.WriteLine("")
        Console.WriteLine("                                                 ( ) Spielen")
        Console.WriteLine("                  ,_~````~-,                     ( ) Anleitung")
        Console.WriteLine("                 .'(_)------`,                   ( ) Scoreboard")
        Console.WriteLine("                 |===========|                   ( ) Charakter/ Spieleranzahl ändern")
        Console.WriteLine("                 `,---------,'                   ( ) Namen ändern")
        Console.WriteLine("                   ~-.___.-~                     ( ) Credits")
        Console.WriteLine("                                                 ( ) Spiel schließen")

        'Do
        ' Benutzereingabe abfragen                                                  
        ' Spielmodus auswähle

        Do
            'Cursor anzeigen:
            Console.SetCursorPosition(50, zeile)
            Console.WriteLine(">")
            taste = Tastatur_Abfrage()
            If taste = CURSOR_DOWN Then
                If zeile < 16 Then
                    zeile = zeile + 1
                End If
            End If
            If taste = CURSOR_UP Then
                If zeile > 10 Then
                    zeile = zeile - 1
                End If
            End If
            'Alte Cursor löschen:
            Console.SetCursorPosition(50, zeile - 1)
            Console.WriteLine(" ")
            Console.SetCursorPosition(50, zeile + 1)
            Console.WriteLine(" ")
        Loop Until taste = KEY_ENTER

        If zeile = 10 Then
            Console.Clear()
            Console.CursorVisible = False
            Call untermenu()
        ElseIf zeile = 11 Then
            Console.Clear()
            Console.CursorVisible = False
            Call Anleitung()
        ElseIf zeile = 12 Then
            Console.Clear()
            Console.CursorVisible = False
            Call scoreboard()
        ElseIf zeile = 13 Then
            Console.Clear()
            Console.CursorVisible = False
            Call auswahl()
        ElseIf zeile = 14 Then
            Console.Clear()
            Console.CursorVisible = False
            Call NamenAendern()
        ElseIf zeile = 15 Then
            Console.Clear()
            Console.CursorVisible = False
            Console.WriteLine("Credits")
            Console.ReadKey()
            Call menu()
        ElseIf zeile = 16 Then
            Console.Clear()
            Console.CursorVisible = False
            End
        End If
        '    choice1 = Console.ReadKey

        '    Select Case choice1
        '        Case "1"
        '            Console.Clear()
        '            Call untermenu()
        '        Case "2"
        '            Console.Clear()
        '            Call Anleitung()
        '        Case "3"
        '            Console.Clear()
        '            Call scoreboard()
        '        Case "4"
        '            Console.Clear()
        '            Call auswahl()
        '        Case "5"
        '            Console.Clear()
        '            Call NamenAendern()
        '        Case "6"
        '            Console.WriteLine("Credits")
        '            Thread.Sleep(1000)
        '        Case "7"
        '            Console.Clear()
        '            Console.WriteLine("Schade! Bis zum nächsten Mal!")
        '            Thread.Sleep(1000)
        '        Case Else
        '            Console.Clear()
        '            Console.WriteLine("Ungültige Auswahl!")
        '            Thread.Sleep(2000)
        '            Console.Clear()
        '    End Select
        'Loop Until choice1 = "7"
        'Loop Until taste = KEY_1 Or taste = KEY_2 Or taste = KEY_3 Or taste = KEY_4 Or taste = KEY_5 Or taste = KEY_6 Or taste = KEY_7
        'End
    End Sub

    Sub NamenAendern()
        Console.CursorVisible = True
        Console.Clear()
        Console.SetCursorPosition(0, 10)
        Console.WriteLine("                           Alles klar, gib einen neuen Namen ein und drücke 'ENTER':                                   ")
        Console.WriteLine("                                                  ->                                ")
        Console.SetCursorPosition(52, 11)
        Namen = Console.ReadLine()
        Console.WriteLine("                                           Name erfolgreich geändert.")
        Console.CursorVisible = False
        Thread.Sleep(1500)
        Console.Clear()
        Call menu()
    End Sub

    Sub scoreboard()
        Dim lesen As String
        Dim punkte() As Integer
        Dim namen() As String
        Dim zeilen() As String
        Console.Clear()
        Console.SetCursorPosition(2, 2)
        lesen = My.Computer.FileSystem.ReadAllText("highscore.txt")
        'zeilen = Split(lesen, vbCrLf)
        'Sort(punkte, namen)
        Console.WriteLine(lesen)
        Console.ReadKey()
        Console.Clear()
        Call menu()
    End Sub

    'Sub Sort(ByRef array As Integer(), ByRef array2 As String())
    '    For i = 0 To array.Length - 1
    '        Dim current As Integer = array(i)
    '        Dim current2 As String = array2(i)
    '        Dim j As Integer = i - 1
    '        While j >= 0
    '            If array(j) > current Then
    '                array(j + 1) = array(j)
    '                array2(j + 1) = array2(j)
    '                j = j - 1
    '            Else
    '                Exit While
    '            End If
    '        End While
    '        array(j + 1) = current
    '        array2(j + 1) = current2
    '    Next
    'End Sub

    Sub untermenu()
        'Menu für das Einstellen der Schwierigkeit
        Dim schwierigkeit As String
        Dim taste As Integer
        Dim zeile As Integer = 10

        Console.CursorVisible = False
        Console.SetCursorPosition(0, 8)
        Console.WriteLine("                                     Wie stark bist du in der Macht?")
        Console.WriteLine("")
        Console.WriteLine("                 .-.                             ( ) Jüngling ")
        Console.WriteLine("                '_8_'                            ( ) Meister")
        Console.WriteLine("               .| |o|.                           ( ) Großmeister")
        Console.WriteLine("               ||___||                           ( ) Zurück zum Hauptmenu")
        Console.WriteLine("               |J   L|")
        'Do

        'If taste = KEY_1 Then                                               
        '    Console.Clear()
        '    Einstellung = "einfach"
        '    Call Spielablauf()
        'ElseIf taste = KEY_2 Then
        '    Einstellung = "mittel"
        '    Call Spielablauf()
        'ElseIf taste = KEY_3 Then
        '    Einstellung = "schwer"
        '    Call Spielablauf()
        'ElseIf taste = KEY_4 Then
        '    Console.WriteLine("Zurück zum Hauptmenu...")
        '    Thread.Sleep(1500)
        '    Call menu()
        'End If

        Do
            Console.SetCursorPosition(50, zeile)
            Console.WriteLine(">")
            taste = Tastatur_Abfrage()
            If taste = CURSOR_DOWN Then
                If zeile < 13 Then
                    zeile = zeile + 1
                End If
            End If
            If taste = CURSOR_UP Then
                If zeile > 10 Then
                    zeile = zeile - 1
                End If
            End If
            Console.SetCursorPosition(50, zeile - 1)
            Console.WriteLine(" ")
            Console.SetCursorPosition(50, zeile + 1)
            Console.WriteLine(" ")
        Loop Until taste = KEY_ENTER

        '    schwierigkeit = Console.ReadLine

        '    Select Case schwierigkeit
        '        Case "1"
        '            Einstellung = "einfach"
        '            Call Spielablauf()
        '        Case "2"
        '            Einstellung = "mittel"
        '            Call Spielablauf()
        '        Case "3"
        '            Einstellung = "schwer"
        '            Call Spielablauf()
        '        Case "4"
        '            Console.WriteLine("Zurück zum Hauptmenu...")
        '            Thread.Sleep(1500)
        '        Case Else
        '            Console.WriteLine("Ungültige Auswahl!")
        '    End Select
        'Loop Until schwierigkeit = "4"
        'Loop Until taste = KEY_1 Or taste = KEY_2 Or taste = KEY_3 Or taste = KEY_4
        'Loop Until zeile = 10 Or zeile = 11 Or zeile = 12 Or zeile = 13

        If zeile = 10 Then
            Console.Clear()
            Einstellung = "einfach"
            Console.CursorVisible = False
            Call Spielablauf()
        ElseIf zeile = 11 Then
            Console.Clear()
            Einstellung = "mittel"
            Console.CursorVisible = False
            Call Spielablauf()
        ElseIf zeile = 12 Then
            Console.Clear()
            Einstellung = "schwer"
            Console.CursorVisible = False
            Call Spielablauf()
        ElseIf zeile = 13 Then
            Console.Clear()
            Call menu()
        End If
    End Sub

    Sub Vorspann()
        'Vorspann vor Auswahlmenü
        Console.BackgroundColor = ConsoleColor.Black
        Console.ForegroundColor = ConsoleColor.Cyan
        Console.CursorVisible = False
        Console.SetCursorPosition(0, 10)
        Console.WriteLine("                                    Es war einmal vor langer Zeit")
        Console.WriteLine("                                    In einer weit weit entfernten Galaxis...")
        Thread.Sleep(500)
        Console.Clear()

        My.Computer.Audio.Play("Star wars.wav", AudioPlayMode.BackgroundLoop)

        Console.ForegroundColor = ConsoleColor.DarkYellow
        Console.BackgroundColor = ConsoleColor.Black

        'Console.SetCursorPosition(0, 4)
        'Console.WriteLine("                              :###**********************%  *%******%*      %#**********#%*")
        'Console.WriteLine("                             +%                         %  %        %      %             *%")
        'Console.WriteLine("                             %.     *###%%%%#    *******% %+   #%   -%     %    *%**#%.   #*")
        'Console.WriteLine("                             =%   .%.      .%    %       -%   .%%+   %+    %    *%***#:   %=")
        'Console.WriteLine("                             :%*    #*     .%    %       %    %+ %    %    %            -%#")
        'Console.WriteLine("                   ***********#%-    =%    .%    %      #*    ****.   +%   %    ::     =%%**")
        'Console.WriteLine("                  =%                 .%    .%    %     .%              %-  %    =%%:   **")
        'Console.WriteLine("                   =%               .%:    .%    %     %:   %%%%%%%%    %  %    =% *#.   *** ")
        'Console.WriteLine("                  :%***************-+       %****%    -%***#%      ##***%= %#***#*   =#*********%:")
        'Console.WriteLine("                   %#***%. .%****%  -%***%#  :%******%:      ##**********#*        +#**********%.")
        'Console.WriteLine("                   =%   .% %:    -% %    %:  %.      .%      %:            %*    .%:        %-")
        'Console.WriteLine("                    %=   #%%      %%*   *%  ##   *#   ##     %:   :%%%%%    %+   %-     :%:")
        'Console.WriteLine("                     %    %        %    %  .%    %%    %.    %:   =%:::%*   *#   #%    %+")
        'Console.WriteLine("                     *%       #*       %=  %-   %*=%   :%    %:    ::::.   +%     *%:   %=")
        'Console.WriteLine("                      %:     .%%      =%  +%    ****    %*   %:    .     =%%:::::::*%=    *#")
        'Console.WriteLine("                      :%     %--%     %   %              %   %:   -%#      ::::::::::      %")
        'Console.WriteLine("                       %*   *%  %-   %*  %+   ##****##   =%  %-   -%-%*                   *%")
        'Console.WriteLine("                        %**#%    %**#%  -%***#%      %#***%= %%***%%  :%%***************%%+")
        'Thread.Sleep(5000)
        'Console.Clear()

        Console.SetCursorPosition(0, 4)
        Console.WriteLine("                              .               .    .          .              .   .         .")
        Console.WriteLine("                                _________________      ____         __________")
        Console.WriteLine("                  .       .    /                 |    /    \    .  |          \")
        Console.WriteLine("                      .       /    ______   _____| . /      \      |    ___    |     .     .")
        Console.WriteLine("                              \    \    |   |       /   /\   \     |   |___>   |")
        Console.WriteLine("                            .  \    \   |   |      /   /__\   \  . |         _/               .")
        Console.WriteLine("                  .     ________>    |  |   | .   /            \   |   |\    \_______    .")
        Console.WriteLine("                       |            /   |   |    /    ______    \  |   | \           |")
        Console.WriteLine("                       |___________/    |___|   /____/      \____\ |___|  \__________|    .")
        Console.WriteLine("                   .     ____    __  . _____   ____      .  __________   .  _________")
        Console.WriteLine("                        \    \  /  \  /    /  /    \       |          \    /         |      .")
        Console.WriteLine("                         \    \/    \/    /  /      \      |    ___    |  /    ______|  .")
        Console.WriteLine("                          \              /  /   /\   \ .   |   |___>   |  \    \")
        Console.WriteLine("                    .      \            /  /   /__\   \    |         _/.   \    \            +")
        Console.WriteLine("                            \    /\    /  /            \   |   |\    \______>    |   .")
        Console.WriteLine("                             \  /  \  /  /    ______    \  |   | \              /          .")
        Console.WriteLine("                  .       .   \/    \/  /____/      \____\ |___|  \____________/  ")
        Console.WriteLine("                                                .                                        .")
        Thread.Sleep(500)
        Console.Clear()



        Console.SetCursorPosition(0, 10)
        Console.WriteLine("                           .___________. _______  __   __               ___   ___")
        Console.WriteLine("                           |           ||   ____||  | |  |              \  \ /  /")
        Console.WriteLine("                           `---|  |----`|  |__   |  | |  |               \  V  /")
        Console.WriteLine("                               |  |     |   __|  |  | |  |                >   <")
        Console.WriteLine("                               |  |     |  |____ |  | |  `----.          /  .  \")
        Console.WriteLine("                               |__|     |_______||__| |_______|         /__/ \__\")
        Thread.Sleep(500)
        Console.Clear()

        Console.SetCursorPosition(0, 9)
        Console.WriteLine("                       ____       _     _            _     _       _   _ _")
        Console.WriteLine("                      / ___|  ___| |__ | | __ _  ___| |__ | |_    (_) (_) |__   ___ _ __")
        Console.WriteLine("                      \___ \ / __| '_ \| |/ _` |/ __| '_ \| __|   | | | | '_ \ / _ \ '__|")
        Console.WriteLine("                      _)  | (__| | | | | (_| | (__| | | | |_  |   |_| | |_) | | __/ | |")
        Console.WriteLine("                   ___|____/ \___|_| |_|_|\__,_|\___|_| |_|\__|    \__,_|_.__/ \___|_|")
        Console.WriteLine("                  |  ___| __(_) ___  __| |_ __(_) ___| |__  ___| |__   __ _ / _| ___ _ __")
        Console.WriteLine("                  | |_ | '__| |/ _ \/ _` | '__| |/ __| '_ \/ __| '_ \ / _` | |_ / _ \ '_ \")
        Console.WriteLine("                  |  _|| |  | |  __/ (_| | |  | | (__| | | \__ \ | | | (_| |  _|  __/ | | |")
        Console.WriteLine("                  |_|  |_|  |_|\___|\__,_|_|  |_|\___|_| |_|___/_| |_|\__,_|_|  \___|_| |_|")
        Thread.Sleep(500)
        Console.Clear()

        'Call text()
        Call auswahl()
    End Sub

    Sub auswahl()
        'Spieler kann Spielcharakter auswählen
        Dim choice As String
        Console.SetCursorPosition(0, 3)
        Console.WriteLine("                            Wähle einen Charakter mit dem du Spielen willst.")
        Console.WriteLine("                      Tippe die jeweilige Zahl ein und drücke anschließend 'ENTER'.")
        Console.WriteLine("")
        Console.WriteLine("                    Yoda (1)                                              Darth Vader (2)")
        Console.WriteLine("")
        Console.WriteLine("        Yoda benutzt den Millenium Falcon                 Darth Vader benutzt seinen eigenen TIE-Fighter")
        Console.WriteLine("")
        Console.WriteLine("                                    __________________________________")
        Console.WriteLine("                                    |:                           ``::%H|")
        Console.WriteLine("                                    |%:.       Starfighters         `:%|")
        Console.WriteLine("                                    |H%::..___________________________:|")
        Console.WriteLine("                     _ .                                                     ")
        Console.WriteLine("                  __CL\H--.                                                 ")
        Console.WriteLine("                L__/_\H' \\--_-                                               / _  _ \")
        Console.WriteLine("                __L_(=): ]-_ _-- -                ->                         |=(_)(_)=|")
        Console.WriteLine("                T__\ /H. //---- -                                             \   *  /")
        Console.WriteLine("                     ~^-H--' ")
        Console.WriteLine("                       *")
        Console.WriteLine("")
        Console.WriteLine("   Für Infos über die Starfighter drücke (3) für Daten über den Millenium Falcon und (4) für den Tie-Fighter.")

        If CharaAuswahl = 0 Then
            'My.Computer.Audio.Play("tie.wav", AudioPlayMode.WaitToComplete)                 'Nur beim ersten mal öffnen wird die Vader Audio abgespielt
            'My.Computer.Audio.Play("vader2.wav", AudioPlayMode.WaitToComplete)
            My.Computer.Audio.Play("Battle of the Heroes.wav", AudioPlayMode.BackgroundLoop)
        ElseIf CharaAuswahl > 0 Then
            My.Computer.Audio.Play("tie.wav", AudioPlayMode.WaitToComplete)
            My.Computer.Audio.Play("Battle of the Heroes.wav", AudioPlayMode.BackgroundLoop)
        End If
        Console.CursorVisible = True
        Do
            ' Benutzereingabe abfragen
            Console.SetCursorPosition(52, 17)
            choice = Console.ReadLine()
            ' Spielmodus auswählen
            Select Case choice
                Case "1"
                    CharaAuswahl = CharaAuswahl + 1
                    Console.Clear()
                    Charakter = "Yoda"
                    'Call yoda()
                Case "2"
                    CharaAuswahl = CharaAuswahl + 1
                    Console.Clear()
                    Charakter = "Darth Vader"
                    'Call Vader()
                Case "3"
                    CharaAuswahl = CharaAuswahl + 1
                    Console.Clear()
                    'Call DatenYoda()
                Case "4"
                    CharaAuswahl = CharaAuswahl + 1
                    Console.Clear()
                    'Call DatenVader()
                Case Else
                    Console.Clear()
                    Console.SetCursorPosition(20, 12)
                    Console.CursorVisible = False
                    CharaAuswahl = CharaAuswahl + 1
                    Console.WriteLine("      Ungültige Auswahl! Bitte gebe eine Zahl zwischen 1 und 4 ein.")
                    Thread.Sleep(1500)
                    Console.Clear()
                    Call auswahl()
            End Select
        Loop Until choice = "1" Or choice = "2" Or choice = "3" Or choice = "4"
        Console.CursorVisible = False
        If choice = "1" Then
            Call yoda()
        ElseIf choice = "2" Then
            Call Vader()
        ElseIf choice = "3" Then
            Call DatenYoda()
        ElseIf choice = "4" Then
            Call DatenVader()
        End If
    End Sub

    Sub Multiplayer_Auswahl()
        Dim choice As String
        Console.CursorVisible = True
        Do

            Console.Clear()
            Console.SetCursorPosition(0, 10)
            Console.WriteLine("                         Möchtest du alleine oder den Multiplayer-Modus spielen?")
            Console.WriteLine()
            Console.WriteLine("        Wenn du alleine spielen willst drücke (1), für den Multiplayer-Modus die (2) und dann 'ENTER'.")
            Console.WriteLine()
            Console.WriteLine("                 Sei dir bewusst, dass der Mulitplayer ein Spaßmodus ist und kein Score ")
            Console.WriteLine("            im Scoreboard gespeichert wird. Es gewinnt lediglich derjenige, der länger überlebt! ")
            Console.WriteLine("")
            Console.WriteLine("                                                  ->    ")
            Console.SetCursorPosition(52, 17)
            choice = Console.ReadLine

            Select Case choice
                Case "1"
                    Multiplayer = 0
                    Console.Clear()
                Case "2"
                    Multiplayer = Multiplayer + 1
                    Console.Clear()
                Case Else
                    Console.Clear()
                    Console.SetCursorPosition(20, 12)
                    Console.WriteLine("      Ungültige Auswahl! Bitte gebe eine Zahl zwischen 1 und 2 ein.")
                    Thread.Sleep(1500)
                    Console.Clear()
            End Select
        Loop Until choice = "1" Or choice = "2"

        Call menu()
    End Sub
    Sub Anleitung()
        Console.CursorVisible = False
        Console.SetCursorPosition(0, 2)
        Console.WriteLine("                                            === Anleitung ===")
        Console.WriteLine("")
        Console.WriteLine("")
        Console.WriteLine("  Ziel des Spiels: Versuche so lange wie möglich den herunterkommenden Hindernissen")
        Console.WriteLine("                   mit deiner Spielfigur auszuweichen. Dabei wird dein Score am unteren Spielfeldrand angezeigt.")
        Console.WriteLine("                   Du kannst deine Figur dabei nach rechts, links und begrenzt nach oben und unten bewegen.")
        Console.WriteLine("")
        Console.WriteLine("")
        Console.WriteLine("        Spielmodi: Es gibt drei verschiedene Spielmodi, einfach (Jüngling), mittel (Meister)")
        Console.WriteLine("                   und schwer (Großmeister). Da du nach jeder Runde zurück ins Hauptmenü kommst, kannst")
        Console.WriteLine("                   du den Spielmodus jederzeit ändern.")
        Console.WriteLine("")
        Console.WriteLine("")
        Console.WriteLine("            Pause: Es gibt die Möglichkeit das Spiel zu pausieren.")
        Console.WriteLine("                   Drücke einfach die Taste 'P' auf der Tastatur.")
        Console.WriteLine("")
        Console.WriteLine("")
        Console.WriteLine("      Multiplayer: Beim Mutliplayer wird der Score nicht gespeichert. Spieler '1' steuert")
        Console.WriteLine("                   seine Figur über die Cursor-Tasten, Spieler '2' über die 'WASD'-Tasten.")
        Console.WriteLine("")
        Console.WriteLine("                                                     '1'")
        Console.WriteLine("                Drücke eine beliebige Taste, um auf die Zweite Seite der Anleitung zu kommen.")
        Console.ReadKey()
        Console.Clear()
        Console.SetCursorPosition(0, 2)
        Console.WriteLine("                                            === Anleitung ===")
        Console.WriteLine("")
        Console.WriteLine("")
        Console.WriteLine("          Gadgets: In jeder Zeile erscheinen, neben der Hindernisse, zusätzliche Gadgets welche dir durchaus")
        Console.WriteLine("                   Vorteile bringen können. Aber sei gewarnt! Die Gadgets können auch inmitten eines Hindernis")
        Console.WriteLine("                   erscheinen. Es gibt dabei bei vier verschiede Gadgets:")
        Console.WriteLine("")
        Console.WriteLine("")
        Console.WriteLine("              '2': Durch das Berühren der '2' wird dein aktueller Score mit 2 multipliziert.")
        Console.WriteLine("")
        Console.WriteLine("              '5': Durch das Berühren der '5' wird deinem aktuellen Score 5 dazu addiert.")
        Console.WriteLine("")
        Console.WriteLine("              '|': Durch das Berühren des 'S' Kannst du einen Schuss abfeuern. Drücke dafür die Leertaste.")
        Console.WriteLine("                   Unter dem Score werden dir deine restlichen Schüsse angezeigt. Ein Schuss ist dabei je")
        Console.WriteLine("                   nach Charakter in der Weite begrenzt, durchtrifft aber alles was ihm in den Weg kommt.")
        Console.WriteLine("                   Wie weit der jeweilige Charakter schießen kann, entnimmst du aus den Daten im Auswahl-Menü")
        Console.WriteLine("                   Sei dir bewusst, dass im Multiplayer kein Schuss verfügbar ist.")
        Console.WriteLine("")
        Console.WriteLine("              '♥': Durch Kontakt mit dem Herzen erhälst du ein Leben mehr. Diese werden die am unteren ")
        Console.WriteLine("                   Bildschirmrand angezeigt.")
        Console.WriteLine("")
        Console.WriteLine("                                                     '2'")
        Console.WriteLine("                Drücke eine beliebige Taste, um in das Hauptmenü zurück zu kommen.")
        Console.ReadKey()
        Console.Clear()
        Call menu()
    End Sub

    Sub Vader()
        Console.ForegroundColor() = ConsoleColor.Red
        Console.SetCursorPosition(0, 2)
        Console.WriteLine("                           GUTE WAHL! WIR WERDEN FRIEDRICHSHAFEN VERNICHTEN!")
        Console.WriteLine("                                               ")
        Console.WriteLine("                                                .-.")
        Console.WriteLine("                                               |_:_|")
        Console.WriteLine("                                              /(_Y_)\")
        Console.WriteLine("                                            ( \/ M \/ )")
        Console.WriteLine("                           .               _.*-/*-*\-*._")
        Console.WriteLine("                             :           _/.--`[[[[]`--.\_")
        Console.WriteLine("                               :        /_`  : |::`| :  `.\ ")
        Console.WriteLine("                                 :     //   ./ |oUU| \.`  :\ ")
        Console.WriteLine("                                 `:  _:`..`  \_|___|_/ :   :| ")
        Console.WriteLine("                                    `:.  .`  |_[___]_|  :.`:\ ")
        Console.WriteLine("                                        [\ |    | |  :   ; : \")
        Console.WriteLine("                                      `-`   \/`.| |.` \  .;.` |")
        Console.WriteLine("                                      |\_    \  `-`   :       |")
        Console.WriteLine("                                      |  \    \ .:    :   |   |")
        Console.WriteLine("                                      |   \    | `.   :    \  |")
        Console.WriteLine("                                    /       \   :. .;       |")
        Console.WriteLine("                                   /     |   |  :__/     :    \\")
        Console.WriteLine("                                   |  |   |    \:   | \   |   ||")
        Console.WriteLine("                                  /    \  : :  |:   /  |__|   /|")
        Console.WriteLine("                                  |     : : :_/_|  /`._\  `--|_\")
        Console.WriteLine("                                       /___.-/_|-`   \  \")
        Console.WriteLine("                                                `-`")
        'My.Computer.Audio.Play("vader.wav", AudioPlayMode.WaitToComplete)
        Thread.Sleep(1000)
        Console.Clear()
        Call Multiplayer_Auswahl()
    End Sub

    Sub yoda()
        Console.ForegroundColor = ConsoleColor.Green
        Console.SetCursorPosition(0, 7)
        Console.WriteLine("                               TU ES! WIR MÜSSEN FRIEDRICHSHAFEN RETTEN!")
        Console.WriteLine("")
        Console.WriteLine("                                                 ____")
        Console.WriteLine("                                              _.' :  `._")
        Console.WriteLine("                                          .-.'`.  ;   .'`.-.")
        Console.WriteLine("                                 __      / : ___\ ;  /___ ; \      __")
        Console.WriteLine("                               ,'_ ``--.:__;``.- .`;: :`.-.`:__;.--`` _`,")
        Console.WriteLine("                               :' `.t``--.. '<@.`;_  ',@>` ..--``j.' `;")
        Console.WriteLine("                                    `:-.._J '-.-'L__ `-- ' L_..-;'")
        Console.WriteLine("                                      ` - .__ ;  .-`  `-.  : __.-`")
        Console.WriteLine("                                          L ' /.------.\ ' J")
        Console.WriteLine("                                           ` - .   `--`   .-`")
        Console.WriteLine("                                          __.l` -: _JL_;-`;.__")
        Console.WriteLine("                                       .-j/'.;  ;````  / .'\`-.")
        Console.WriteLine("                                     .` /:`.  -.:     .-  .`;  `.")
        'My.Computer.Audio.Play("yoda.wav", AudioPlayMode.WaitToComplete)
        Console.Clear()
        Call Multiplayer_Auswahl()
    End Sub

    Sub ladebalken()
        Console.CursorVisible = False
        Console.SetCursorPosition(0, 5)
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
        Console.Clear()
        Console.CursorVisible = True
        Console.SetCursorPosition(0, 10)
        Console.WriteLine("                       Herzlich Willkommen! Gib deinen Namen ein und schon geht es gleich los: ")
        Console.SetCursorPosition(52, 11)
        Console.WriteLine("-> ")
        Console.SetCursorPosition(55, 11)
        Namen = Console.ReadLine()
        Console.Clear()
        Console.CursorVisible = False
        Console.SetCursorPosition(0, 7)
        Console.WriteLine("  Danke. Bevor es losgeht ein Hinweis. Es gibt einzelne Soundeffekte und Überblendungen zwischen den Sequenzen.")
        Console.WriteLine("        Während allen Soundeffekten (ausgenommen der Hintergrundmusik) solltest du keine Tasten drücken,")
        Console.WriteLine("     da diese sonst erst verzögert angezeigt werden. Warte also immer bis du einen Cursor siehst oder eine ")
        Console.WriteLine("                            andere Aufforderung erhälst, wie zum Beispiel jetzt! ;)")
        Console.WriteLine("")
        Console.WriteLine("                             Drücke eine beliebige Taste um das Spiel zu starten.")
        Console.ReadKey()
        Console.Clear()
    End Sub

    Sub text()
        Console.SetCursorPosition(0, 4)
        Console.WriteLine("                     .     .            .        .          .       .                  .      .")
        Console.WriteLine("                   .     .                                                        .")
        Console.WriteLine("                               .   Düstere Zeiten sind angebrochen. Es ist   .        .     .")
        Thread.Sleep(3000)
        Console.WriteLine("                                  Krieg! Die dunkle Macht bedroht Friedrichs   .  .")
        Thread.Sleep(3000)
        Console.WriteLine("                      .       .  hafen mitsamt seinen Einwohnern. Die Bürger            .")
        Thread.Sleep(3000)
        Console.WriteLine("                 .        .     sind in Sorge und Fürchten sich zwecks der nahen                  .")
        Thread.Sleep(3000)
        Console.WriteLine("                    .          Bedrohung.   .      .")
        Thread.Sleep(3000)
        Console.WriteLine("                              Niemand anderes als der stärkste Krieger der Zeit, ")
        Thread.Sleep(3000)
        Console.WriteLine("                           . Darth Vader, möchte sein Imperium mit Hilfe seiner  .   .")
        Thread.Sleep(3000)
        Console.WriteLine("                    .       Raumschiffflotte ausbauen. Die Stadt Friedrichshafen ist        .  .")
        Thread.Sleep(3000)
        Console.WriteLine("                 .      .  dabei bei den Imperialisten extremst begehrt. Doch als eine   .")
        Thread.Sleep(3000)
        Console.WriteLine("                          der letzten verbleibenden, uneingenommenen Regionen wird sich             .")
        Thread.Sleep(3000)
        Console.WriteLine("                     .   Friedrichshafen bis zum Ende wehren.   .    .")
        Thread.Sleep(3000)
        Console.WriteLine("                 .      Dabei bekommen die tapferen Einwohner Hilfe von niemand anderem als     .")
        Thread.Sleep(3000)
        Console.WriteLine("                     . dem Jedi-Großmeister Yoda selbst.")
        Thread.Sleep(3000)
        Console.WriteLine("                      Er ist die große Hoffnung und Unterstützung, die es benötigt die dunkle            .")
        Thread.Sleep(3000)
        Console.WriteLine("                     Seite aufzuhalten.  .")
        Thread.Sleep(3000)
        Console.WriteLine("")
        Console.WriteLine("                  . Werden es die Einwohner schaffen Friedrichshafen erfolgreich zu verteidigen")
        Thread.Sleep(3000)
        Console.WriteLine("                   oder wird schon bald das Böse über die Stadt herrschen.")
        Thread.Sleep(3000)
        Console.WriteLine("                  Nur DU kannst es herausfinden! Denn die Macht ist stark in dir! ALSO LOS GEHTS!             .         .")
        Thread.Sleep(3000)
        Console.WriteLine("                 .        .          .    .    .            .            .                   .")
        Thread.Sleep(3000)
        Thread.Sleep(4000)
        Console.Clear()
    End Sub

    Sub DatenYoda()
        Console.ForegroundColor = ConsoleColor.Green
        Console.SetCursorPosition(0, 7)
        Console.WriteLine("                                                                  _ .")
        Console.WriteLine("                                                             __CL\H--.")
        Console.WriteLine("                                                          L__/_\H' \\--_-")
        Console.WriteLine("                                                           __L_(=): ]-_ _-- -")
        Console.WriteLine("                                                          T__\ /H. //---- -")
        Console.WriteLine("                                                              ~^-H--'")
        Console.WriteLine("                                                                 `")
        Console.SetCursorPosition(2, 2)
        Console.WriteLine("Der Millenium-Falcon:")
        Console.WriteLine()
        Console.WriteLine()
        My.Computer.Audio.Play("tiping.wav", AudioPlayMode.Background)
        Dim satz1 As String = "  Top-Speed: 9,31- fache Lichtgeschwindigkeit "
        For Each buchstabe As Char In satz1
            Console.Write(buchstabe)
            Thread.Sleep(40)
        Next
        Console.WriteLine()
        Console.WriteLine()
        Dim satz2 As String = "  Länge: 34,75 Meter"
        For Each buchstabe As Char In satz2
            Console.Write(buchstabe)
            Thread.Sleep(40)
        Next
        Console.WriteLine()
        Console.WriteLine()
        Dim satz3 As String = "  Stärke: 2 Militärische Schutzschilde und"
        For Each buchstabe As Char In satz3
            Console.Write(buchstabe)
            Thread.Sleep(40)
        Next
        Console.WriteLine()
        Dim satz4 As String = "  1 Antierschütterungsschild"
        For Each buchstabe As Char In satz4
            Console.Write(buchstabe)
            Thread.Sleep(40)
        Next
        Console.WriteLine()
        Console.WriteLine()
        Dim satz5 As String = "  Schussweite: 5"
        For Each buchstabe As Char In satz5
            Console.Write(buchstabe)
            Thread.Sleep(40)
        Next
        Console.WriteLine()
        Console.WriteLine()
        Dim satz6 As String = "  Vorteile: Durch die Starke Verteidigung kann der"
        For Each buchstabe As Char In satz6
            Console.Write(buchstabe)
            Thread.Sleep(40)
        Next
        Console.WriteLine()
        Dim satz7 As String = "  Millenium-Falke viel mehr aushalten als andere Raumschiffe"
        For Each buchstabe As Char In satz7
            Console.Write(buchstabe)
            Thread.Sleep(40)
        Next
        Console.WriteLine()
        Console.WriteLine()
        Console.WriteLine()
        Console.WriteLine()
        Console.ForegroundColor = ConsoleColor.DarkYellow
        Console.WriteLine("  Drücke eine beliebige Taste um zur Auswahl zurückzukehren.")
        My.Computer.Audio.Play("jedi.wav", AudioPlayMode.BackgroundLoop)
        Console.ReadKey()
        Console.Clear()
        Call auswahl()
    End Sub

    Sub DatenVader()
        Console.ForegroundColor = ConsoleColor.DarkRed
        Console.SetCursorPosition(0, 8)
        Console.WriteLine("                                                              / _  _ \")
        Console.WriteLine("                                                             |=(_)(_)=|")
        Console.WriteLine("                                                              \   *  /")
        Console.SetCursorPosition(2, 2)
        Console.WriteLine("Der TIE-Fighter:")
        Console.WriteLine()
        Console.WriteLine()
        My.Computer.Audio.Play("tiping.wav", AudioPlayMode.Background)
        Dim satz1 As String = "  Top-Speed: 1250km/h"
        For Each buchstabe As Char In satz1
            Console.Write(buchstabe)
            Thread.Sleep(40)
        Next
        Console.WriteLine()
        Console.WriteLine()
        Dim satz2 As String = "  Länge: 9 Meter"
        For Each buchstabe As Char In satz2
            Console.Write(buchstabe)
            Thread.Sleep(40)
        Next
        Console.WriteLine()
        Console.WriteLine()
        Dim satz3 As String = "  Stärke: Personalisierte Laserkanonen für "
        For Each buchstabe As Char In satz3
            Console.Write(buchstabe)
            Thread.Sleep(40)
        Next
        Console.WriteLine()
        Dim satz4 As String = "  Darth Vader"
        For Each buchstabe As Char In satz4
            Console.Write(buchstabe)
            Thread.Sleep(40)
        Next
        Console.WriteLine()
        Console.WriteLine()
        Dim satz5 As String = "  Schussweite: 6"
        For Each buchstabe As Char In satz5
            Console.Write(buchstabe)
            Thread.Sleep(40)
        Next
        Console.WriteLine()
        Console.WriteLine()
        Dim satz6 As String = "  Vorteile: Durch die Macht und die Laserkanonen kann der"
        For Each buchstabe As Char In satz6
            Console.Write(buchstabe)
            Thread.Sleep(40)
        Next
        Console.WriteLine()
        Dim satz7 As String = "  TIE-Fighter weiter schießen und ein wenig schneller punkten"
        For Each buchstabe As Char In satz7
            Console.Write(buchstabe)
            Thread.Sleep(40)
        Next
        Console.WriteLine()
        Console.WriteLine()
        Console.WriteLine()
        Console.WriteLine()
        Console.ForegroundColor = ConsoleColor.DarkYellow
        Console.WriteLine("  Drücke eine beliebige Taste um zur Auswahl zurückzukehren.")
        My.Computer.Audio.Play("sith.wav", AudioPlayMode.BackgroundLoop)
        Console.ReadKey()
        Console.Clear()
        Call auswahl()
    End Sub

    Sub Main()
        Console.BackgroundColor = ConsoleColor.Black
        Console.SetWindowSize(115, 300)
        Console.Clear()
        Console.CursorVisible = False
        Call ladebalken()
        Call Vorspann()
        'Call auswahl()
        'Call Spielablauf()
        'Call Multiplayer_Auswahl()
        'Call menu()
        'Call menu()
        'Call Spielablauf()

        Console.ReadLine()
    End Sub

End Module
