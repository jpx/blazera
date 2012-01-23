Element_DragonStatue = Element()
Element_DragonStatue:SetSkin(Create:Texture("Element_DragonStatue1"))
Element_DragonStatue:AddBoundingBox(BBoundingBox(Element_DragonStatue, 5, 33, 58, 59))
--[[Element_DragonStatue:AddOpacityBox(OpacityBox(
                FloatRect(
                    5,
                    20,
                    60,
                    59), EDesactivatingSideType.Bottom))--]]
return Element_DragonStatue