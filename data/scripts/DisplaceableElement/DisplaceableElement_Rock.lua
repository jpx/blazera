--> eronne <--

DisplaceableElement_Rock = DisplaceableElement()
DisplaceableElement_Rock.Skin = Create:Texture("DisplaceableElement_Rock")
DisplaceableElement_Rock.Name = "ROCK"
vit = 20

top = EBoundingBox(DisplaceableElement_Rock, EBoundingBoxType.Body, 3, -2, 17, 2)
top.OnInEvent = function(source, trigger) PlayerHdl:Cl("Top") source.Holder:EnableDirection(Direction.S) end
top.OnOutEvent = function(source, trigger) PlayerHdl:Cl("Top") source.Holder:DisableDirection(Direction.S) end
DisplaceableElement_Rock:AddBoundingBox(top)

left = EBoundingBox(DisplaceableElement_Rock, EBoundingBoxType.Body, -2, 3, 2, 17)
left.OnInEvent = function(source, trigger) PlayerHdl:Cl("Left") source.Holder:EnableDirection(Direction.E) end
left.OnOutEvent = function(source, trigger) PlayerHdl:Cl("Left") source.Holder:DisableDirection(Direction.E) end
DisplaceableElement_Rock:AddBoundingBox(left)

bottom = EBoundingBox(DisplaceableElement_Rock, EBoundingBoxType.Body, 3, 17, 17, 22)
bottom.OnInEvent = function(source, trigger) PlayerHdl:Cl("Bottom") source.Holder:EnableDirection(Direction.N) end
bottom.OnOutEvent = function(source, trigger) PlayerHdl:Cl("Bottom") source.Holder:DisableDirection(Direction.N) end
DisplaceableElement_Rock:AddBoundingBox(bottom)

right = EBoundingBox(DisplaceableElement_Rock, EBoundingBoxType.Body, 17, 3, 22, 17)
right.OnInEvent = function(source, trigger) PlayerHdl:Cl("Right") source.Holder:EnableDirection(Direction.O) end
right.OnOutEvent = function(source, trigger) PlayerHdl:Cl("Right") source.Holder:DisableDirection(Direction.O) end
DisplaceableElement_Rock:AddBoundingBox(right)

DisplaceableElement_Rock:AddBoundingBox(BBoundingBox(DisplaceableElement_Rock, 0, 7, 20, 20))
return DisplaceableElement_Rock