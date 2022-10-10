Imports System.IO
Imports System.Threading
Imports System.Threading.Tasks

Public Class Form1
    Dim dir As Double
    Dim turnrate As Double

    Dim bmp As Bitmap
    Dim Bg, Img As CImage
    Dim SpriteMap As CImage
    Dim SpriteMask As CImage
    Dim CPIntro, CPRSlashFade, CPRSlashJump, CPFeatAttFade, CPFeatAttInit, CPDis, CPStag, CPShoot, CPFloat, CPStand As CArrFrame
    Dim MMStand, MMStagger, MMDisappear, MMShoot As CArrFrame
    Dim MMPlasma, MMProjDisappear As CArrFrame
    Dim CPProjLock, CPProjHoming1, CPProjHoming2, CPProjHoming3, CPProjHoming4, CPProjHoming5, CPProjExplode, CPOpenCH, CPProjDis, CPHomingCH As CArrFrame
    Dim TargetAcq As Boolean = False
    Dim CreateBull As Boolean = False
    Dim BulletCreated As Boolean = False

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.S Then
            MsgBox("test")
            If MM.CurrState = StateMegaMan.Stand Then
                MM.State(StateMegaMan.Shoot, 3)
                CreatePlasma()
            End If
        End If
    End Sub

    Dim BulletHit As Boolean = False


    Dim ListChar As New List(Of CCharacter)
    Dim SM As CCharCyberPeacock
    Dim SP As CCharCPProjectile
    Dim CB As CCharCPProjectile
    Dim MM As CCharMegaMan
    Dim MP As CCharMMProjectile
    Dim PlasmaCreated As Boolean = False

    Dim V As Double

    Function CollisionDetection(ByVal frame1 As CElmtFrame, ByVal frame2 As CElmtFrame, ByVal object1 As CCharacter, ByVal object2 As CCharacter)
        Dim L1, L2, R1, R2, T1, T2, B1, B2 As Integer

        L1 = frame1.Left - frame1.CtrPoint.x + object1.PosX
        R1 = frame1.Right - frame1.CtrPoint.x + object1.PosX
        T1 = frame1.Top - frame1.CtrPoint.y + object1.PosY
        B1 = frame1.Bottom - frame1.CtrPoint.y + object1.PosY

        L2 = frame2.Left - frame2.CtrPoint.x + object2.PosX
        R2 = frame2.Right - frame2.CtrPoint.x + object2.PosX
        T2 = frame2.Top - frame2.CtrPoint.y + object2.PosY
        B2 = frame2.Bottom - frame2.CtrPoint.y + object2.PosY

        If L2 < R1 And L1 < R2 And T1 < B2 And T2 < B1 Then
            Return True
        Else
            Return False
        End If

    End Function

    Sub updatehoming(ByVal x1 As Double, ByVal y1 As Double, ByVal destx As Double, ByVal desty As Double)
        Dim dx, dy As Double
        Dim vx, vy As Double
        Dim dist As Double
        Dim z As Double

        dx = destx
        dy = desty

        'stop condition
        dist = (destx - x1) * (destx - x1) + (desty - y1) * (desty - y1)

        V = 5
        If dist >= 400 Then
            vx = V * Math.Cos(dir)
            vy = V * Math.Sin(dir)

            'check turn left or right
            'get vector d
            dx = dx - x1
            dy = dy - y1

            'get Z
            z = vx * dy - vy * dx

            If z >= 0 Then 'turn left
                dir = dir + turnrate
            Else 'turn right
                dir = dir - turnrate
            End If

            'update v
            vx = V * Math.Cos(dir)
            vy = V * Math.Sin(dir)

            If TargetAcq = True Then
                CB.PosX = CB.PosX + vx
                CB.PosY = CB.PosY + vy
            Else
                SP.PosX = SP.PosX + vx
                SP.PosY = SP.PosY + vy
            End If

            'draw
            PictureBox1.Refresh()
        Else
            TargetAcq = True
            CreateBull = True
        End If




    End Sub

    Private Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        If SM.CurrState = StateCP.Disappear Then
            If SM.PosX < 337 Then
                SM.PosX = 100
                SM.FDir = FaceDir.Right
            Else
                SM.PosX = 570
                SM.FDir = FaceDir.Left
            End If
            SM.State(StateCP.Float, 8) 'end up in stand
            TargetAcq = False
            BulletCreated = False
            CreateBull = False
        End If

    End Sub

    Private Sub RadioButton1_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles RadioButton1.CheckedChanged

    End Sub

    Private Sub PictureBox1_MouseClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PictureBox1.MouseClick

        If SM.CurrState = StateCP.Disappear Then
            SM.PosX = e.X
            If RadioButton1.Checked Then
                SM.State(StateCP.RisingSlashFade, 1)
            ElseIf RadioButton2.Checked Then
                SM.State(StateCP.FeatherAttackFade, 3)
            End If
        End If
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'variable for homing
        dir = 210 * Math.PI / 180
        turnrate = 4 * Math.PI / 180

        'open image for background, assign to bg
        RadioButton2.Checked = True
        Randomize()

        Bg = New CImage
        Bg.OpenImage(("C: \Users\Raymond\Desktop\University\Semester 8\CGA\Animation Sprites\BG.bmp"))

        Bg.CopyImg(Img)

        SpriteMap = New CImage
        SpriteMap.OpenImage(("C:\Users\Raymond\Desktop\University\Semester 8\CGA\Animation Sprites\CP_2.bmp"))

        SpriteMap.CreateMask(SpriteMask)

        'initialize sprites for CyberPeacock
        CPIntro = New CArrFrame
        'first row on sprite sheet
        CPIntro.Insert(50, 38, 7, 6, 83, 71, 1)
        CPIntro.Insert(126, 38, 88, 6, 164, 71, 1)
        CPIntro.Insert(207, 38, 169, 6, 245, 71, 1)
        CPIntro.Insert(288, 38, 250, 6, 326, 71, 1)
        CPIntro.Insert(369, 38, 331, 4, 407, 71, 1)
        CPIntro.Insert(450, 37, 412, 4, 488, 71, 1)
        CPIntro.Insert(531, 37, 493, 4, 569, 71, 1)
        CPIntro.Insert(612, 38, 575, 5, 650, 71, 1)
        CPIntro.Insert(693, 38, 656, 5, 731, 71, 1)
        CPIntro.Insert(774, 38, 736, 5, 812, 71, 1)
        'second row on sprite sheet
        CPIntro.Insert(44, 129, 7, 96, 82, 162, 1)
        CPIntro.Insert(126, 129, 88, 96, 164, 162, 1)
        CPIntro.Insert(207, 124, 169, 91, 245, 162, 1)
        CPIntro.Insert(288, 124, 251, 90, 326, 162, 1)
        CPIntro.Insert(369, 121, 331, 84, 407, 162, 1)
        CPIntro.Insert(450, 121, 412, 84, 488, 162, 1)
        CPIntro.Insert(531, 119, 493, 80, 569, 162, 1)
        CPIntro.Insert(612, 119, 574, 80, 650, 162, 1)
        CPIntro.Insert(693, 116, 656, 75, 731, 162, 1)
        CPIntro.Insert(774, 116, 736, 75, 812, 162, 2)
        'third row of sprite 
        CPIntro.Insert(128, 226, 88, 183, 164, 269, 1)
        CPIntro.Insert(178, 226, 169, 218, 186, 235, 1)
        CPIntro.Insert(215, 226, 191, 203, 240, 252, 1)
        CPIntro.Insert(284, 226, 245, 189, 324, 266, 1)
        CPIntro.Insert(379, 226, 329, 180, 430, 275, 1)
        CPIntro.Insert(492, 226, 435, 172, 550, 283, 1)
        CPIntro.Insert(616, 226, 555, 167, 678, 288, 1)
        CPIntro.Insert(744, 226, 683, 167, 806, 288, 1)
        'fourth row
        CPIntro.Insert(87, 354, 26, 293, 149, 414, 1)
        CPIntro.Insert(215, 354, 154, 293, 277, 414, 1)
        CPIntro.Insert(343, 354, 282, 294, 405, 413, 1)
        CPIntro.Insert(471, 354, 410, 293, 533, 414, 1)
        CPIntro.Insert(599, 354, 538, 294, 661, 413, 1)
        CPIntro.Insert(728, 354, 666, 293, 791, 414, 1)
        'fifth row 
        CPIntro.Insert(43, 465, 4, 419, 82, 511, 1)
        CPIntro.Insert(110, 465, 86, 419, 135, 511, 1)
        CPIntro.Insert(160, 465, 139, 419, 182, 511, 2)
        CPIntro.Insert(205, 465, 186, 419, 224, 511, 2)
        CPIntro.Insert(274, 465, 228, 419, 321, 514, 8)
        CPIntro.Insert(381, 470, 337, 421, 426, 519, 15)
        'Disappear
        CPIntro.Insert(621, 471, 597, 422, 645, 520, 5)

        CPRSlashFade = New CArrFrame
        'disappear
        CPRSlashFade.Insert(621, 471, 597, 422, 645, 520, 5)
        'fadein
        CPRSlashFade.Insert(454, 470, 431, 421, 477, 519, 2)
        CPRSlashFade.Insert(504, 470, 481, 421, 527, 519, 2)
        CPRSlashFade.Insert(454, 470, 431, 421, 477, 519, 2)
        CPRSlashFade.Insert(504, 470, 481, 421, 527, 519, 2)
        CPRSlashFade.Insert(554, 470, 532, 421, 577, 519, 2)
        CPRSlashFade.Insert(454, 470, 431, 421, 477, 519, 2)
        'faded in
        CPRSlashFade.Insert(44, 578, 12, 529, 77, 628, 2)
        CPRSlashFade.Insert(109, 579, 82, 533, 137, 625, 2)
        CPRSlashFade.Insert(177, 579, 151, 533, 203, 626, 2)

        CPRSlashJump = New CArrFrame
        CPRSlashJump.Insert(244, 579, 221, 531, 267, 628, 2)
        'first jump
        CPRSlashJump.Insert(318, 577, 272, 528, 364, 627, 2)
        'second jump
        CPRSlashJump.Insert(416, 577, 371, 528, 462, 627, 2)
        'third jump
        CPRSlashJump.Insert(515, 577, 470, 528, 561, 627, 2)
        'repetition jump
        CPRSlashJump.Insert(318, 577, 272, 528, 364, 627, 2)
        CPRSlashJump.Insert(416, 577, 371, 528, 462, 627, 2)
        CPRSlashJump.Insert(515, 577, 470, 528, 561, 627, 2)
        CPRSlashJump.Insert(318, 577, 272, 528, 364, 627, 2)
        CPRSlashJump.Insert(416, 577, 371, 528, 462, 627, 2)
        CPRSlashJump.Insert(515, 577, 470, 528, 561, 627, 2)
        CPRSlashJump.Insert(318, 577, 272, 528, 364, 627, 2)
        CPRSlashJump.Insert(416, 577, 371, 528, 462, 627, 2)
        CPRSlashJump.Insert(515, 577, 470, 528, 561, 627, 2)
        'disappear
        CPRSlashJump.Insert(621, 471, 597, 422, 645, 520, 5)

        CPFeatAttFade = New CArrFrame

        'fade in
        CPFeatAttFade.Insert(454, 470, 431, 421, 477, 519, 2)
        CPFeatAttFade.Insert(504, 470, 481, 421, 527, 519, 2)
        CPFeatAttFade.Insert(454, 470, 431, 421, 477, 519, 2)
        CPFeatAttFade.Insert(504, 470, 481, 421, 527, 519, 2)
        CPFeatAttFade.Insert(554, 470, 532, 421, 577, 519, 2)
        CPFeatAttFade.Insert(454, 470, 431, 421, 477, 519, 2)

        CPFeatAttInit = New CArrFrame

        CPFeatAttInit.Insert(50, 678, 6, 648, 95, 748, 2)
        CPFeatAttInit.Insert(150, 680, 106, 650, 195, 750, 2)
        CPFeatAttInit.Insert(245, 683, 201, 654, 290, 753, 2)
        CPFeatAttInit.Insert(350, 684, 306, 656, 395, 752, 2)
        CPFeatAttInit.Insert(454, 681, 401, 656, 508, 746, 2)
        CPFeatAttInit.Insert(568, 685, 518, 667, 618, 743, 2)
        CPFeatAttInit.Insert(682, 687, 633, 669, 732, 745, 2)
        'feather attack starts
        CPFeatAttInit.Insert(115, 822, 4, 761, 226, 883, 2)
        CPFeatAttInit.Insert(348, 822, 237, 762, 459, 883, 2)
        CPFeatAttInit.Insert(572, 822, 463, 762, 681, 882, 2)
        CPFeatAttInit.Insert(800, 821, 688, 761, 913, 882, 2)
        CPFeatAttInit.Insert(113, 955, 2, 895, 224, 1015, 2)

        CPDis = New CArrFrame
        CPDis.Insert(621, 471, 597, 422, 645, 520, 5)

        CPStag = New CArrFrame
        CPStag.Insert(44, 1083, 11, 1040, 76, 1130, 1)
        CPStag.Insert(114, 1085, 82, 1040, 145, 1130, 1)
        CPStag.Insert(182, 1085, 151, 1040, 212, 1130, 1)
        CPStag.Insert(248, 1085, 216, 1040, 279, 1130, 1)
        CPStag.Insert(313, 1085, 283, 1040, 343, 1130, 1)

        CPFloat = New CArrFrame
        CPFloat.Insert(454, 470, 431, 421, 477, 519, 2)
        CPFloat.Insert(504, 470, 481, 421, 527, 519, 2)
        CPFloat.Insert(454, 470, 431, 421, 477, 519, 2)
        CPFloat.Insert(504, 470, 481, 421, 527, 519, 2)
        CPFloat.Insert(554, 470, 532, 421, 577, 519, 2)
        CPFloat.Insert(454, 470, 431, 421, 477, 519, 2)
        CPFloat.Insert(46, 1209, 22, 1158, 70, 1258, 5)
        CPFloat.Insert(137, 1209, 87, 1160, 185, 1258, 2)
        CPFloat.Insert(241, 1208, 197, 1160, 289, 1258, 5)

        CPShoot = New CArrFrame
        CPShoot.Insert(56, 1364, 10, 1318, 101, 1411, 1)
        CPShoot.Insert(158, 1370, 111, 1323, 203, 1416, 2)
        CPShoot.Insert(260, 1373, 214, 1327, 306, 1418, 2)
        CPShoot.Insert(364, 1372, 315, 1323, 412, 1417, 2)
        CPShoot.Insert(470, 1372, 422, 1323, 519, 1416, 2)
        CPShoot.Insert(582, 1369, 538, 1324, 624, 1417, 2)

        CPStand = New CArrFrame
        CPStand.Insert(582, 1369, 538, 1324, 624, 1417, 2)

        MMStand = New CArrFrame
        MMStand.Insert(352, 1203, 325, 1163, 380, 1243, 2)

        MMStagger = New CArrFrame
        MMStagger.Insert(435, 1203, 404, 1165, 466, 1241, 1)
        MMStagger.Insert(513, 1205, 482, 1167, 544, 1242, 1)
        MMStagger.Insert(594, 1204, 564, 1166, 624, 1242, 1)
        MMStagger.Insert(673, 1204, 638, 1165, 708, 1242, 1)
        MMStagger.Insert(758, 1205, 723, 1165, 792, 1244, 1)

        MMDisappear = New CArrFrame
        MMDisappear.Insert(621, 471, 597, 422, 645, 520, 5)

        MMShoot = New CArrFrame
        MMShoot.Insert(307, 960, 273, 918, 340, 1001, 1)
        MMShoot.Insert(392, 961, 360, 918, 428, 1003, 1)
        MMShoot.Insert(480, 962, 446, 919, 513, 1003, 1)
        MMShoot.Insert(559, 963, 526, 921, 593, 1004, 1)
        MMShoot.Insert(641, 963, 611, 921, 593, 1004, 1)

        SM = New CCharCyberPeacock
        ReDim SM.ArrSprites(9)
        SM.ArrSprites(0) = CPIntro
        SM.ArrSprites(1) = CPRSlashFade
        SM.ArrSprites(2) = CPRSlashJump
        SM.ArrSprites(3) = CPFeatAttFade
        'not edited
        SM.ArrSprites(4) = CPFeatAttInit
        SM.ArrSprites(5) = CPDis
        SM.ArrSprites(6) = CPShoot
        SM.ArrSprites(7) = CPStag
        SM.ArrSprites(8) = CPFloat
        SM.ArrSprites(9) = CPStand

        SM.PosX = 550
        SM.PosY = 200
        SM.Vx = 0
        SM.Vy = 0
        SM.State(StateCP.Intro, 0)
        SM.FDir = FaceDir.Left

        'MegaMan starts here

        MM = New CCharMegaMan
        ReDim MM.ArrSprites(3)
        MM.ArrSprites(0) = MMStand
        MM.ArrSprites(1) = MMStagger
        MM.ArrSprites(2) = MMDisappear
        MM.ArrSprites(3) = MMShoot

        MM.PosX = 110
        MM.PosY = 320
        MM.State(StateMegaMan.Stand, 0)
        MM.FDir = FaceDir.Right

        ListChar.Add(SM)
        ListChar.Add(MM)

        'initialize sprites for Sprite Projectiles
        CPProjLock = New CArrFrame
        CPProjLock.Insert(517, 1064, 497, 1048, 537, 1080, 2)
        CPProjLock.Insert(560, 1064, 541, 1048, 579, 1080, 2)
        CPProjLock.Insert(603, 1064, 584, 1048, 622, 1080, 2)
        CPProjLock.Insert(646, 1064, 627, 1048, 665, 1080, 2)
        CPProjLock.Insert(689, 1064, 670, 1048, 708, 1080, 2)

        CPProjHoming1 = New CArrFrame
        CPProjHoming1.Insert(374, 1108, 351, 1099, 396, 1117, 1)
        CPProjHoming1.Insert(422, 1108, 400, 1099, 444, 1117, 1)

        CPProjHoming2 = New CArrFrame
        CPProjHoming2.Insert(470, 1108, 449, 1097, 490, 1119, 1)
        CPProjHoming2.Insert(516, 1108, 495, 1097, 537, 1119, 1)

        CPProjHoming3 = New CArrFrame
        CPProjHoming3.Insert(557, 1108, 541, 1091, 573, 1124, 1)
        CPProjHoming3.Insert(594, 1108, 578, 1088, 610, 1127, 1)

        CPProjHoming4 = New CArrFrame
        CPProjHoming4.Insert(626, 1108, 614, 1087, 638, 1128, 1)
        CPProjHoming4.Insert(653, 1107, 642, 1085, 663, 1128, 1)

        CPProjHoming5 = New CArrFrame
        CPProjHoming5.Insert(675, 1106, 667, 1084, 682, 1128, 1)
        CPProjHoming5.Insert(695, 1107, 687, 1084, 702, 1129, 1)

        CPProjExplode = New CArrFrame
        CPProjExplode.Insert(720, 1066, 712, 1059, 727, 1073, 1)
        CPProjExplode.Insert(739, 1066, 730, 1057, 747, 1074, 1)
        CPProjExplode.Insert(760, 1066, 751, 1056, 768, 1075, 1)
        CPProjExplode.Insert(740, 1101, 715, 1079, 765, 1123, 1)

        CPOpenCH = New CArrFrame
        CPOpenCH.Insert(365, 1063, 348, 1061, 365, 1065, 1)

        CPProjDis = New CArrFrame
        CPProjDis.Insert(621, 471, 597, 422, 645, 520, 5)

        CPHomingCH = New CArrFrame
        CPHomingCH.Insert(517, 1064, 497, 1048, 537, 1080, 2)

        MMPlasma = New CArrFrame
        MMPlasma.Insert(803, 1066, 786, 1053, 822, 1076, 1)

        MMProjDisappear = New CArrFrame
        MMProjDisappear.Insert(621, 471, 597, 422, 645, 520, 5)


        bmp = New Bitmap(Img.Width, Img.Height)

        DisplayImg()
        ResizeImg()

        Timer1.Enabled = True
    End Sub

    Sub PutSprites()
        Dim cc As CCharacter

        For i = 0 To Img.Width - 1
            For j = 0 To Img.Height - 1
                Img.Elmt(i, j) = Bg.Elmt(i, j)
            Next
        Next


        For Each cc In ListChar
            Dim EF As CElmtFrame = cc.ArrSprites(cc.IdxArrSprites).Elmt(cc.FrameIdx)
            Dim spritewidth = EF.Right - EF.Left
            Dim spriteheight = EF.Bottom - EF.Top
            If cc.FDir = FaceDir.Left Then
                Dim spriteleft As Integer = cc.PosX - EF.CtrPoint.x + EF.Left
                Dim spritetop As Integer = cc.PosY - EF.CtrPoint.y + EF.Top
                'set mask
                For i = 0 To spritewidth
                    For j = 0 To spriteheight
                        Img.Elmt(spriteleft + i, spritetop + j) = OpAnd(Img.Elmt(spriteleft + i, spritetop + j), SpriteMask.Elmt(EF.Left + i, EF.Top + j))
                    Next
                Next

                'set sprite
                For i = 0 To spritewidth
                    For j = 0 To spriteheight
                        Img.Elmt(spriteleft + i, spritetop + j) = OpOr(Img.Elmt(spriteleft + i, spritetop + j), SpriteMap.Elmt(EF.Left + i, EF.Top + j))
                    Next
                Next
            Else 'facing right
                Dim spriteleft = cc.PosX + EF.CtrPoint.x - EF.Right
                Dim spritetop = cc.PosY - EF.CtrPoint.y + EF.Top
                'set mask
                For i = 0 To spritewidth
                    For j = 0 To spriteheight
                        Img.Elmt(spriteleft + i, spritetop + j) = OpAnd(Img.Elmt(spriteleft + i, spritetop + j), SpriteMask.Elmt(EF.Right - i, EF.Top + j))
                    Next
                Next

                'set sprite
                For i = 0 To spritewidth
                    For j = 0 To spriteheight
                        Img.Elmt(spriteleft + i, spritetop + j) = OpOr(Img.Elmt(spriteleft + i, spritetop + j), SpriteMap.Elmt(EF.Right - i, EF.Top + j))
                    Next
                Next

            End If

        Next
    End Sub

    Sub DisplayImg()
        'display bg and sprite on picturebox
        Dim i, j As Integer
        'Dim sw As New System.Diagnostics.Stopwatch

        'sw.Start()

        PutSprites()

        Dim rect As New Rectangle(0, 0, bmp.Width, bmp.Height)
        Dim bmpdata As System.Drawing.Imaging.BitmapData = bmp.LockBits(rect,
         System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat)

        'Get the address of the first line.
        Dim ptr As IntPtr = bmpdata.Scan0


        'Declare an array to hold the bytes of the bitmap.
        Dim bytes As Integer = Math.Abs(bmpdata.Stride) * bmp.Height
        Dim rgbvalues(bytes) As Byte


        'Copy the RGB values into the array.
        System.Runtime.InteropServices.Marshal.Copy(ptr, rgbvalues, 0, bytes)

        Dim n As Integer = 0
        Dim col As System.Drawing.Color

        'Set every third value to 255. A 24bpp bitmap will look red.  
        For j = 0 To Img.Height - 1
            For i = 0 To Img.Width - 1
                col = Img.Elmt(i, j)
                rgbvalues(n) = col.B
                rgbvalues(n + 1) = col.G
                rgbvalues(n + 2) = col.R
                rgbvalues(n + 3) = col.A

                n = n + 4
            Next
        Next

        'Copy the RGB values back to the bitmap
        System.Runtime.InteropServices.Marshal.Copy(rgbvalues, 0, ptr, bytes)


        'Unlock the bits.
        bmp.UnlockBits(bmpdata)

        'Timer1.Enabled = False

        'MsgBox(CStr(bmp.GetPixel(0, 0).A) + " " + CStr(bmp.GetPixel(0, 0).R) + " " + CStr(bmp.GetPixel(0, 0).G) + " " + CStr(bmp.GetPixel(0, 0).B))
        'MsgBox(CStr(bmp.GetPixel(1, 0).A) + " " + CStr(bmp.GetPixel(1, 0).R) + " " + CStr(bmp.GetPixel(1, 0).G) + " " + CStr(bmp.GetPixel(1, 0).B))

        PictureBox1.Refresh()

        PictureBox1.Image = bmp
        PictureBox1.Width = bmp.Width
        PictureBox1.Height = bmp.Height
        PictureBox1.Top = 0
        PictureBox1.Left = 0

    End Sub

    Sub ResizeImg()
        Dim w, h As Integer

        w = PictureBox1.Width
        h = PictureBox1.Height

        Me.ClientSize = New Size(w, h)
    End Sub


    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Dim CC As CCharacter

        PictureBox1.Refresh()

        For Each CC In ListChar
            CC.Update()
        Next

        'If MM.CurrState = StateMegaMan.Shoot Then
        '    CreatePlasma()
        'End If

        If SM.CurrState = StateCP.Shoot And SM.FrameIdx = 0 Then
            CreateCrosshair()
            Timer1.Interval = 1
        End If

        If SM.CurrState = StateCP.Shoot And SM.FrameIdx <> 0 Then
            updatehoming(SP.PosX, SP.PosY, MM.PosX, MM.PosY)
        ElseIf SM.CurrState = StateCP.Stand And SM.FrameIdx = 0 Then
            updatehoming(SP.PosX, SP.PosY, MM.PosX, MM.PosY)
            If TargetAcq = True And CreateBull = True Then
                If BulletCreated = False Then
                    createBullet()
                End If
            End If
        End If

        If TargetAcq = True And CreateBull = True Then
            If CB.CurrState = StateCPProjectile.Homing1 Then
                updatehoming(CB.PosX, CB.PosY, MM.PosX, MM.PosY)
            End If
        End If

        If BulletCreated = True Then
            If CollisionDetection(MM.ArrSprites(MM.IdxArrSprites).Elmt(MM.FrameIdx), CB.ArrSprites(CB.IdxArrSprites).Elmt(CB.FrameIdx), MM, CB) And (MM.CurrState = StateMegaMan.Stand Or SP.CurrState = StateCPProjectile.Homing1) Then
                MM.State(StateMegaMan.Stagger, 1)
                SM.State(StateCP.Disappear, 5)
                CB.State(StateCPProjectile.Explode, 5)
                'CB.Destroy = True
                SP.Destroy = True
                TargetAcq = False
                BulletCreated = False
                CreateBull = False
            End If
        End If

        If SM.CurrState = StateCP.FeatherAttack Then
            If CollisionDetection(MM.ArrSprites(MM.IdxArrSprites).Elmt(MM.FrameIdx), SM.ArrSprites(SM.IdxArrSprites).Elmt(SM.FrameIdx), MM, SM) And (MM.CurrState = StateMegaMan.Stand Or SM.CurrState = StateCP.FeatherAttack) Then
                MM.State(StateMegaMan.Stagger, 1)
            End If
        End If

        If SM.CurrState = StateCP.RisingSlash Then
            If CollisionDetection(MM.ArrSprites(MM.IdxArrSprites).Elmt(MM.FrameIdx), SM.ArrSprites(SM.IdxArrSprites).Elmt(SM.FrameIdx), MM, SM) And (MM.CurrState = StateMegaMan.Stand Or SM.CurrState = StateCP.RisingSlash) Then
                MM.State(StateMegaMan.Stagger, 1)
            End If
        End If

        If MM.CurrState = StateMegaMan.Shoot Then
            If CollisionDetection(MM.ArrSprites(MM.IdxArrSprites).Elmt(MM.FrameIdx), SM.ArrSprites(SM.IdxArrSprites).Elmt(SM.FrameIdx), MM, SM) And (MM.CurrState = StateMegaMan.Shoot Or SM.CurrState = StateCP.FeatherAttack) Then
                SM.State(StateCP.Stagger, 7)
            End If
        End If

        If PlasmaCreated = True Then
            If CollisionDetection(MP.ArrSprites(MP.IdxArrSprites).Elmt(MP.FrameIdx), SM.ArrSprites(SM.IdxArrSprites).Elmt(SM.FrameIdx), MP, SM) And (MP.CurrState = StateMMProj.Plasma Or SM.CurrState = StateCP.FeatherAttack Or SM.CurrState = StateCP.RisingSlash) Then
                If SM.CurrState <> StateCP.Disappear Then
                    SM.State(StateCP.Stagger, 7)
                End If
            End If
        End If


        Dim Listchar1 As New List(Of CCharacter)

        For Each CC In ListChar
            If Not CC.Destroy Then
                Listchar1.Add(CC)
            End If
        Next

        ListChar = Listchar1

        DisplayImg()
    End Sub

    Sub CreateCrosshair()

        SP = New CCharCPProjectile
        If SM.FDir = FaceDir.Left Then
            SP.PosX = SM.PosX - 25
            SP.FDir = FaceDir.Left
        Else
            SP.PosX = SM.PosX - 25
            SP.FDir = FaceDir.Right
        End If


        SP.PosY = SM.PosY

        SP.CurrState = StateCPProjectile.OpenCH

        ReDim SP.ArrSprites(1)

        SP.ArrSprites(0) = CPOpenCH
        SP.ArrSprites(1) = CPHomingCH

        ListChar.Add(SP)
    End Sub

    Sub createBullet()
        BulletCreated = True
        CB = New CCharCPProjectile
        CB.FDir = SM.FDir
        If CB.FDir = FaceDir.Left Then
            CB.PosX = SM.PosX - 25
            CB.FDir = FaceDir.Left
        Else
            CB.PosX = SM.PosX - 25
            CB.FDir = FaceDir.Right
        End If

        CB.PosY = SM.PosY

        CB.V = 5

        CB.CurrState = StateCPProjectile.formBullet

        ReDim CB.ArrSprites(6)
        CB.ArrSprites(0) = CPProjHoming1 'straight
        CB.ArrSprites(1) = CPProjHoming2 'skewed1
        CB.ArrSprites(2) = CPProjHoming3 '45 angle
        CB.ArrSprites(3) = CPProjHoming4 'skewed2
        CB.ArrSprites(4) = CPProjHoming5 'vertical
        CB.ArrSprites(5) = CPProjExplode
        CB.ArrSprites(6) = CPProjDis

        ListChar.Add(CB)
    End Sub

    Sub CreatePlasma()
        MP = New CCharMMProjectile

        If MM.FDir = FaceDir.Left Then
            MP.PosX = MM.PosX - 15
            MP.FDir = FaceDir.Left
        Else
            MP.PosX = MM.PosX + 15
            MP.FDir = FaceDir.Right
        End If
        MP.PosY = MM.PosY

        MP.CurrState = StateMMProj.Plasma
        PlasmaCreated = True

        ReDim MP.ArrSprites(1)
        MP.ArrSprites(0) = MMPlasma
        MP.ArrSprites(1) = MMProjDisappear
        ListChar.Add(MP)
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If MM.CurrState = StateMegaMan.Stand Then
            MM.State(StateMegaMan.Shoot, 3)
            CreatePlasma()
        End If
    End Sub

End Class
