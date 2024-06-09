Imports System.IO
Imports System.Media
Imports System.Threading

Module Sound

    Public Sub SpieleTonImHintergrundAb(ByRef ton As String)
        Dim soundPlayer = ErstelleSoundPlayer(ton)
        soundPlayer.PlayLooping()
    End Sub

    Public Sub SpieleTonEinmalImHintergrundAb(ByRef ton As String)
        Dim soundPlayer = ErstelleSoundPlayer(ton)
        soundPlayer.Play()
    End Sub

    Public Sub SpieleTonAbUndWarteAufEnde(ByRef ton As String)
        Dim soundPlayer = ErstelleSoundPlayer(ton)
        soundPlayer.PlaySync()
    End Sub

    Public Sub SpieleTonUndDanachSchleife(ByRef einmalTon As String, ByRef schleifeTon As String)
        Dim soundPlayerEinmal = ErstelleSoundPlayer(einmalTon)
        Dim soundPlayerSchleife = ErstelleSoundPlayer(schleifeTon)
        Dim soundThread As New Thread(Sub()
                                          soundPlayerEinmal.PlaySync()
                                          soundPlayerSchleife.PlayLooping()
                                      End Sub)
        soundThread.Start()
    End Sub

    Function ErstelleSoundPlayer(ByRef ton As String) As SoundPlayer
        Dim projektVerzeichnis As String = Directory.GetParent(My.Application.Info.DirectoryPath).Parent.FullName
        Dim soundVerzeichnis As String = Path.Combine(projektVerzeichnis, "resources/Sounds/")
        Dim soundPlayer As New SoundPlayer(soundVerzeichnis + ton + ".wav")
        Return soundPlayer
    End Function

End Module
