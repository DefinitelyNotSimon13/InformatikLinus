' Konstanten die an verschiedenen
Module Konstanten
    ' -- Die Konstanten deutlich zu benennen macht die Kommentare dahinter unnötig 
    Public Const SPALTE_MAX_ANZAHL = 79
    Public Const SPALTE_MIN_ANZAHL = 0
    Public Const ZEILE_MAX_ANZAHL = 24
    Public Const HINDERNIS_MIN_ANZAHL = 2
    Public Const GESCHWINDIGKEIT_SPIELFIGUR = 5

    ' Enum zur Darstellung der möglichen Tasten
    Public Enum Tasten
        None = 0
        Left = 1
        Right = 2
        Unknown = 99
        Space = 3
        Up = 7
        Down = 8
        P = 9
        W = 10
        A = 11
        S = 12
        D = 13
        Y = 14
        V = 15
        Enter = 23
    End Enum

    ' Enum zur Darstellung der möglichen Schwierigkeiten
    Public Enum Schwierigkeiten
        Einfach
        Mittel
        Schwer
    End Enum

    ' Enum zur Darstellung der möglichen Charaktere
    Public Enum Charaktere
        Yoda
        DarthVader
    End Enum
End Module
