Element_StoneStatue = Element()
Element_StoneStatue:SetSkin(Create:Texture("Element_StoneStatue"))
Element_StoneStatue:AddBoundingBox(BBoundingBox(Element_StoneStatue, 6, 41, 27, 58))
Element_StoneStatue:AddOpacityBox(OpacityBox(
                FloatRect(
                    0,
                    0,
                    32,
                    16)))

Element_StoneStatue:AddOpacityBox(OpacityBox(
                FloatRect(
                    0,
                    16,
                    32,
                    64),
                EDesactivatingSideType.All))
return Element_StoneStatue