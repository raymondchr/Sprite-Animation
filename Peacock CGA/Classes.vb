Imports System.IO

Public Enum StateCP
    Intro
    RisingSlashFade
    RisingSlash
    FeatherAttackFade
    FeatherAttack
    Disappear
    Stagger
    Shoot
    Float
    Stand
End Enum

Public Enum StateCPProjectile
    Lock
    formBullet
    Homing1
    Homing2
    Homing3
    Homing4
    Homing5
    Explode
    OpenCH
    HomingCH
    Dissappear
End Enum

Public Enum StateMegaMan
    Stand
    Stagger
    Disappear
    Shoot
End Enum

Public Enum StateMMProj
    Plasma
    Disappear
End Enum

Public Enum FaceDir
    Left
    Right
End Enum

Public Class CImage
    Public Width As Integer
    Public Height As Integer
    Public Elmt(,) As System.Drawing.Color
    Public ColorMode As Integer 'not used

    Sub OpenImage(ByVal FName As String)
        'open file .bmp, read file
        Dim s As String
        Dim L As Long
        Dim BR As BinaryReader
        Dim h, w, pos As Integer
        Dim r, g, b As Integer
        Dim pad As Integer

        BR = New BinaryReader(File.Open(FName, FileMode.Open))

        Try
            BlockRead(BR, 2, s)

            If s <> "BM" Then
                MsgBox("Not a BMP file")
            Else 'BMP file
                BlockReadInt(BR, 4, L) 'size
                'MsgBox("Size = " + CStr(L))
                BlankRead(BR, 4) 'reserved
                BlockReadInt(BR, 4, pos) 'start of data
                BlankRead(BR, 4) 'size of header
                BlockReadInt(BR, 4, Width) 'width
                'MsgBox("Width = " + CStr(I.Width))
                BlockReadInt(BR, 4, Height) 'height
                'MsgBox("Height = " + CStr(I.Height))
                BlankRead(BR, 2) 'color panels
                BlockReadInt(BR, 2, ColorMode) 'colormode
                If ColorMode <> 24 Then
                    MsgBox("Not a 24-bit color BMP")
                Else

                    BlankRead(BR, pos - 30)

                    ReDim Elmt(Width - 1, Height - 1)
                    pad = (4 - (Width * 3 Mod 4)) Mod 4

                    For h = Height - 1 To 0 Step -1
                        For w = 0 To Width - 1
                            BlockReadInt(BR, 1, b)
                            BlockReadInt(BR, 1, g)
                            BlockReadInt(BR, 1, r)
                            Elmt(w, h) = Color.FromArgb(r, g, b)

                        Next
                        BlankRead(BR, pad)

                    Next

                End If

            End If

        Catch ex As Exception
            MsgBox("Error")

        End Try

        BR.Close()


    End Sub


    Sub CreateMask(ByRef Mask As CImage)
        'create mask from *this*
        Dim i, j As Integer

        Mask = New CImage
        Mask.Width = Width
        Mask.Height = Height

        ReDim Mask.Elmt(Mask.Width - 1, Mask.Height - 1)

        For i = 0 To Width - 1
            For j = 0 To Height - 1
                If Elmt(i, j).R = 0 And Elmt(i, j).G = 0 And Elmt(i, j).B = 0 Then
                    Mask.Elmt(i, j) = Color.FromArgb(255, 255, 255)
                Else
                    Mask.Elmt(i, j) = Color.FromArgb(0, 0, 0)
                End If
            Next
        Next

    End Sub


    Sub CopyImg(ByRef Img As CImage)
        'copies image to Img
        Img = New CImage
        Img.Width = Width
        Img.Height = Height
        ReDim Img.Elmt(Width - 1, Height - 1)

        For i = 0 To Width - 1
            For j = 0 To Height - 1
                Img.Elmt(i, j) = Elmt(i, j)
            Next
        Next

    End Sub

End Class

Public Class CCharacter
    Public PosX, PosY As Double
    Public Vx, Vy As Double
    Public FrameIdx As Integer
    Public CurrFrame As Integer
    Public ArrSprites() As CArrFrame
    Public IdxArrSprites As Integer
    Public FDir As FaceDir
    Public Destroy As Boolean = False
    Public V As Integer

    Public Const gravity = 1

    'Public CurrState as ?

    Public Sub GetNextFrame()
        CurrFrame = CurrFrame + 1
        If CurrFrame = ArrSprites(IdxArrSprites).Elmt(FrameIdx).MaxFrameTime Then
            FrameIdx = FrameIdx + 1
            If FrameIdx = ArrSprites(IdxArrSprites).N Then
                FrameIdx = 0
            End If
            CurrFrame = 0

        End If

    End Sub

    Public Overridable Sub Update()

    End Sub


End Class



Public Class CCharCyberPeacock
    Inherits CCharacter

    Public CurrState As StateCP

    Public Sub State(ByVal state As StateCP, ByVal idxspr As Integer)
        CurrState = state
        IdxArrSprites = idxspr
        CurrFrame = 0
        FrameIdx = 0

    End Sub

    Public Overrides Sub Update()
        Select Case CurrState
            Case StateCP.Intro
                GetNextFrame()
                If FrameIdx = 0 And CurrFrame = 0 Then
                    State(StateCP.Disappear, 5)
                End If

            Case StateCP.Disappear
                GetNextFrame()


            Case StateCP.RisingSlashFade
                PosY = 300

                If Vx <= 24 Then
                    Vx += 2
                Else

                End If

                GetNextFrame()

                If FrameIdx = 0 And CurrFrame = 0 Then
                    State(StateCP.RisingSlash, 2)
                    Vy = -20
                    Vx = 0
                End If

            Case StateCP.RisingSlash
                PosY = PosY + Vy
                GetNextFrame()

                If Vy = 0 Then
                    State(StateCP.Disappear, 5)

                    PosY = 300
                Else
                    Vy = Vy + gravity
                End If

            Case StateCP.FeatherAttackFade
                PosY = 300

                Vx += 2
                GetNextFrame()
                If FrameIdx = 0 And CurrFrame = 0 Then
                    State(StateCP.FeatherAttack, 4)
                    Vx = 0
                End If

            Case StateCP.FeatherAttack
                GetNextFrame()
                If FrameIdx = 0 And CurrFrame = 0 Then
                    State(StateCP.Disappear, 5)
                End If

            Case StateCP.Stagger
                GetNextFrame()
                If FrameIdx = 0 And CurrFrame = 0 Then
                    State(StateCP.Disappear, 5)
                End If

            Case StateCP.Float
                GetNextFrame()
                PosY = 200
                If FrameIdx = 0 And CurrFrame = 0 Then
                    State(StateCP.Shoot, 6)
                End If

            Case StateCP.Shoot
                GetNextFrame()
                If FrameIdx = 0 And CurrFrame = 0 Then
                    State(StateCP.Stand, 9)
                End If

            Case StateCP.Stand
                GetNextFrame()



        End Select

    End Sub
End Class

Public Class CCharMegaMan
    Inherits CCharacter

    Public CurrState As StateMegaMan

    Public TargetX As Double
    Public TargetY As Double

    Public Sub State(ByVal state As StateMegaMan, ByVal idxspr As Integer)
        CurrState = state
        IdxArrSprites = idxspr
        CurrFrame = 0
        FrameIdx = 0
    End Sub

    Public Overrides Sub Update()
        Select Case CurrState
            Case StateMegaMan.Stand
                GetNextFrame()

            Case StateMegaMan.Stagger
                GetNextFrame()
                If FrameIdx = 0 And CurrFrame = 0 Then
                    State(StateMegaMan.Disappear, 2)
                    Dim MyValue As Integer

                    MyValue = Int((500 * Rnd()) + 1)
                    PosX = MyValue
                End If

            Case StateMegaMan.Disappear
                GetNextFrame()
                If FrameIdx = 0 And CurrFrame = 0 Then
                    State(StateMegaMan.Stand, 0)
                End If

            Case StateMegaMan.Shoot
                GetNextFrame()
                If FrameIdx = 0 And CurrFrame = 0 Then
                    State(StateMegaMan.Stand, 0)
                End If

        End Select


    End Sub
End Class

Public Class CCharCPProjectile
    Inherits CCharacter

    Public CurrState As StateCPProjectile

    Public TargetX As Double
    Public TargetY As Double

    Public Sub State(ByVal state As StateCPProjectile, ByVal idxspr As Integer)
        CurrState = state
        IdxArrSprites = idxspr
        CurrFrame = 0
        FrameIdx = 0

    End Sub

    Public Overrides Sub Update()
        Select Case CurrState

            Case StateCPProjectile.OpenCH
                GetNextFrame()
                If FrameIdx = 0 And CurrFrame = 0 Then
                    State(StateCPProjectile.HomingCH, 1)
                End If

            Case StateCPProjectile.HomingCH
                GetNextFrame()

                'End of Crosshair
                'Bullet will start here vvvvv

            Case StateCPProjectile.formBullet
                GetNextFrame()
                If FrameIdx = 0 And CurrFrame = 0 Then
                    State(StateCPProjectile.Homing1, 0)
                End If

            Case StateCPProjectile.Homing1
                GetNextFrame()

            Case StateCPProjectile.Explode
                GetNextFrame()
                If FrameIdx = 0 And CurrFrame = 0 Then
                    Destroy = True
                End If


        End Select
    End Sub

End Class

Public Class CCharMMProjectile
    Inherits CCharacter

    Public CurrState As StateMMProj

    Public Sub State(ByVal state As StateMMProj, ByVal idxspr As Integer)
        CurrState = state
        IdxArrSprites = idxspr
        CurrFrame = 0
        FrameIdx = 0

    End Sub

    Public Overrides Sub Update()
        Select Case CurrState
            Case StateMMProj.Plasma
                GetNextFrame()
                If FDir = FaceDir.Right Then
                    PosX = PosX + 5
                    If PosX > 600 Then
                        Destroy = True
                    Else
                        State(StateMMProj.Plasma, 0)
                    End If

                ElseIf FDir = FaceDir.Left Then
                    PosX = PosX - 5
                    If PosX < 95 Then
                        Destroy = True
                    End If
                End If

            Case StateMMProj.Disappear
                GetNextFrame()

        End Select
    End Sub

End Class


Public Class CElmtFrame
    Public CtrPoint As TPoint
    Public Top, Bottom, Left, Right As Integer

    Public MaxFrameTime As Integer

    Public Sub New(ByVal ctrx As Integer, ByVal ctry As Integer, ByVal l As Integer, ByVal t As Integer, ByVal r As Integer, ByVal b As Integer, ByVal mft As Integer)
        CtrPoint.x = ctrx
        CtrPoint.y = ctry
        Top = t
        Bottom = b
        Left = l
        Right = r
        MaxFrameTime = mft

    End Sub
End Class

Public Class CArrFrame
    Public N As Integer
    Public Elmt As CElmtFrame()

    Public Sub New()
        N = 0
        ReDim Elmt(-1)
    End Sub

    Public Overloads Sub Insert(ByVal E As CElmtFrame)
        ReDim Preserve Elmt(N)
        Elmt(N) = E
        N = N + 1
    End Sub

    Public Overloads Sub Insert(ByVal ctrx As Integer, ByVal ctry As Integer, ByVal l As Integer, ByVal t As Integer, ByVal r As Integer, ByVal b As Integer, ByVal mft As Integer)
        Dim E As CElmtFrame
        E = New CElmtFrame(ctrx, ctry, l, t, r, b, mft)
        ReDim Preserve Elmt(N)
        Elmt(N) = E
        N = N + 1

    End Sub

End Class

Public Structure TPoint
    Dim x As Integer
    Dim y As Integer

End Structure
