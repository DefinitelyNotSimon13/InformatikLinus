' Funktionen zur Erstellung der einzelnen Gadgets die dargestellt werden
Module SpielElemente

    'Prozedur zur Erzeugung eines ♥ in der Zeile
    Public Sub ErzeugeHerz(ByRef zeile() As Char)
        ErzeugeSymbol(zeile, "♥")
    End Sub

    'Prozedur zur Erzeugung eines +5 Punkt in der Zeile
    Public Sub Erzeuge5Punkte(ByRef zeile() As Char)
        ErzeugeSymbol(zeile, "5")
    End Sub

    'Prozedur zur Erzeugung *2 in der Zeile
    Public Sub Erzeuge2Punkte(ByRef zeile() As Char)
        ErzeugeSymbol(zeile, "2")
    End Sub

    ' Prozedur zur Erzeugung eines Schusses in der Zeile
    Public Sub ErzeugeSchussDrop(ByRef Zeile() As Char)
        ErzeugeSymbol(Zeile, "|")
    End Sub

    'Prozedur zur Erzeugung eines Symbols in einer Zeile
    Sub ErzeugeSymbol(ByRef zeile() As Char, symbol As Char)
        Dim P As Integer                'Startposition des Symbols
        Dim x As Single                 'Zufallszahl
        'Startposition des Symbols zufällig festlegen:
        x = VBMath.Rnd
        P = ((SPALTE_MAX_ANZAHL - SPALTE_MIN_ANZAHL) * x) + SPALTE_MIN_ANZAHL
        For j = 1 To 1
            'Prüfen, ob das Symbol innerhalb des Vektors liegt
            If P + j - 1 <= SPALTE_MAX_ANZAHL Then
                'Symbol an der Position P + j - 1 in den Vektor schreiben
                zeile(P + j - 1) = symbol
            End If
        Next
    End Sub

    Public Function ErzeugeZeile(ByVal hindernisMaxAnzahl As Integer) As Char()
        Dim anzahlHindernisse As Integer
        Dim groesseHindernis As Integer
        Dim startPositionHindernis As Integer
        Dim zeile As Char()

        'Zeilenvektor mit Leerzeichen vorbelegen:
        zeile = Enumerable.Repeat(Of Char)(" ", SPALTE_MAX_ANZAHL + 1).ToArray()

        'Festlegung der Hindernisgrößen basierend auf der Schwierigkeitseinstellung
        Select Case Schwierigkeit
            Case Schwierigkeiten.Einfach
                MinGroesseHinderniss = 1
                MaxGroesseHinderniss = 1
            Case Schwierigkeiten.Mittel
                MinGroesseHinderniss = 5
                MaxGroesseHinderniss = 8
            Case Schwierigkeiten.Schwer
                MinGroesseHinderniss = 8
                MaxGroesseHinderniss = 14
        End Select


        'Anzahl der Hindernisblocks zufällig festlegen:
        anzahlHindernisse = ((hindernisMaxAnzahl - HINDERNIS_MIN_ANZAHL) * VBMath.Rnd) + HINDERNIS_MIN_ANZAHL

        For i As Integer = 1 To anzahlHindernisse

            'Größe G des Hindernisblocks zufällig festlegen:
            groesseHindernis = ((MaxGroesseHinderniss - MinGroesseHinderniss) * VBMath.Rnd) + MinGroesseHinderniss

            'Startposition des Hindernisblocks zufällig festlegen:
            startPositionHindernis = ((SPALTE_MAX_ANZAHL - SPALTE_MIN_ANZAHL) * VBMath.Rnd) + SPALTE_MIN_ANZAHL

            'Für jedes der G Einzelhindernisse, Laufvariable:
            For j As Integer = 1 To groesseHindernis
                'Prüfen, ob das Hindernis innerhalb des Vektors liegt
                If startPositionHindernis + j - 1 <= SPALTE_MAX_ANZAHL Then
                    'Hindernis an der Startposition + j - 1 in den Vektor schreiben
                    zeile(startPositionHindernis + j - 1) = "■"
                End If
            Next
        Next

        Return zeile
    End Function

End Module
