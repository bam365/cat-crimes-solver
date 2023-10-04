module Problems 

open Game

let problem4: Logic = And(
    [ Fact(NumberOfCatsPresent(3))
    ; catAcrossFromCat Ginger Sassy
    ; catPlacesFromCat Sassy 1 MrMittens
    ; catPlacesFromPlace Ginger 1 Birdcage
    ; Fact(PlacesRightOf(set [Ginger], 1, Cats(set [MrMittens])))
    ]
)

let problem6 = And(
    [ Fact(InFront ( set [NoCat],  Set.intersect (placesWith BellBall) (placesWith PawPrint) ))
    ; Fact(InFront ( set [NoCat],  Set.intersect (placesWith Catnip) (placesWith ClawMarks) ))
    ; catInFrontOfItem TomCat Mouse
    ; catInFrontOfItem Duchess Sock
    ; catPlacesFromCat Ginger 1 Sassy
    ; catPlacesFromPlace Ginger 1 Fishbowl
    ; catAcrossFromCat Ginger TomCat
    ]
)

let problem7 = And(
    [ catInFrontOfPlace MrMittens Fishbowl
    ; Fact(PlacesLeftOf(set [Sassy], 1, Cats(set [MrMittens])))
    ; catInFrontOfItem TomCat ClawMarks
    ; Fact(PlacesFrom(set [Ginger], 2, Cats(set [Sassy]))) 
    ; Fact(PlacesRightOf(set [Ginger], 1, Cats(set [Duchess])))
    ; catAcrossFromCat PipSqueak Duchess
    ]
)

let problem8: Logic = And(
    [ catInFrontOfPlace Sassy Birdcage
    ; Fact(PlacesRightOf(set [MrMittens], 1, Cats(set [Ginger])))
    ; Not(catPlacesFromCat Ginger 1 Sassy)
    ; Fact(PlacesRightOf(set [Ginger], 1, Cats(set [TomCat])))
    ; between (set [Duchess]) (Cats(set [TomCat])) (Cats(set [PipSqueak]))
    ]
)

let problem9: Logic = And(
    [ catInFrontOfPlace TomCat Birdcage
    ; catPlacesFromCat Sassy 2 TomCat
    ; Or(
        [ catPlacesFromCat Ginger 1 MrMittens
        ; catPlacesFromCat Ginger 1 PipSqueak
        ]
    )
    ; Fact(PlacesRightOf(set [NoCat], 1, Cats(set [Ginger])))
    ; Or(
        [ Fact(InFront ( set [MrMittens],  Set.intersect (placesWith BellBall) (placesWith PawPrint) ))
        ; Fact(InFront ( set [MrMittens],  Set.intersect (placesWith Catnip) (placesWith ClawMarks) ))
        ]
    )
    ]
)

let problem13: Logic = And(
    [ catAcrossFromCat MrMittens Duchess
    ; catAcrossFromCat PipSqueak Sassy
    ; catInFrontOfItem TomCat BellBall
    ; catInFrontOfItem Sassy BellBall
    ; catInFrontOfItem Ginger Catnip
    ; catPlacesFromCat TomCat 3 MrMittens
    ]
)

let problem29: Logic = And(
    [ Fact(PlacesLeftOf(set [Ginger], 1, Cats(set [Sassy])))
    ; catPlacesFromCat MrMittens 1 Duchess
    ; Not(catPlacesFromCat PipSqueak 1 TomCat)
    ; Not(catAcrossFromCat Sassy MrMittens)
    ; catAcrossFromCat TomCat Duchess 
    ; Not(catPlacesFromPlace TomCat 1 Fishbowl)
    ; allCatsPresent
    ]
)

let problem30: Logic = And(
    [ catPlacesFromCat Ginger 2 Sassy
    ; catAcrossFromCat Ginger MrMittens
    ; Fact(PlacesFrom(set [Ginger], 3, Cats(catsWith LongHair)))
    ; Fact(PlacesRightOf(set [NoCat], 1, Cats(set [Ginger])))
    ; Or(
        [ catInFrontOfItem TomCat ClawMarks
        ; catInFrontOfItem TomCat BellBall
        ]
    ) 
    ; exclusiveOr
        (Fact(PlacesRightOf(set [Ginger], 1, Cats(set [TomCat]))))
        (catInFrontOfItem Ginger Catnip)
    ]
)
