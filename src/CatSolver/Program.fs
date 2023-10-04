﻿open System

type Cat =
    | PipSqueak
    | Duchess
    | TomCat
    | Ginger
    | MrMittens
    | Sassy
    | NoCat

type CatProperty =
    | WhitePaws
    | BlueEyes
    | Bow
    | Bell
    | GreenEyes
    | LongHair

type Place =
    | Birdcage
    | Coffee
    | Shoes
    | Fishbowl
    | Yarn
    | Plant

type Item = 
    | Sock
    | Catnip
    | Mouse
    | BellBall
    | ClawMarks
    | PawPrint

type CatsOrPlaces =
    | Cats of Set<Cat>
    | Places of Set<Place>

type Fact =
    | NumberOfCatsPresent of int
    | CatsPresent of Set<Cat>
    | InFront of Set<Cat> * Set<Place>
    | AcrossFrom of Set<Cat> * CatsOrPlaces
    | PlacesFrom of Set<Cat> * int * CatsOrPlaces
    | PlacesLeftOf of Set<Cat> * int * CatsOrPlaces
    | PlacesRightOf of Set<Cat> * int * CatsOrPlaces

type Logic =
    | Fact of Fact
    | Not of Logic
    | And of Logic list
    | Or of Logic list

type Configuration = Map<Place, Cat>


let catsWith catProp = 
    match catProp with
    | WhitePaws -> [Sassy; MrMittens]
    | BlueEyes -> [TomCat; Ginger]
    | Bell -> [MrMittens; PipSqueak]
    | Bow -> [Duchess; TomCat]
    | GreenEyes -> [Sassy; PipSqueak]
    | LongHair -> [Duchess; Sassy]
    |> Set.ofList

let placesWith item =
    match item with
    | Sock -> [Plant; Fishbowl]
    | Catnip -> [Plant; Shoes]
    | Mouse -> [Birdcage; Yarn]
    | BellBall -> [Coffee; Fishbowl]
    | PawPrint -> [Coffee; Yarn]
    | ClawMarks -> [Shoes; Birdcage]
    |> Set.ofList 

let placeAcrossFrom = function
    | Birdcage -> Fishbowl
    | Coffee -> Plant
    | Shoes -> Yarn
    | Fishbowl -> Birdcage
    | Yarn -> Shoes
    | Plant -> Coffee

let placeNumber = function
    | Birdcage -> 0
    | Coffee -> 1
    | Shoes -> 2
    | Fishbowl -> 3
    | Yarn -> 4
    | Plant -> 5

let numberPlace n =
    let normalized = 
        if n < 0 then 6 - ((abs n) % 6) 
        else n % 6
    match normalized with
    | 0 -> Birdcage
    | 1 -> Coffee
    | 2 -> Shoes
    | 3 -> Fishbowl
    | 4 -> Yarn
    | 5 -> Plant
    | _ -> failwith "oops, you messed up your normalization"
        
let placesLeft n place = 
    numberPlace ((placeNumber place) + n)

let placesRight n place = 
    numberPlace ((placeNumber place) - n)

let placesAway n place =
    [ placesLeft n place; placesRight n place ]

let placesNextTo = placesAway 1 

let catAtPlace (configuration: Configuration) place = Map.find place configuration

let catPlaces (configuration: Configuration) cats =
    configuration
    |> Map.filter (fun k v -> Set.contains v cats)
    |> Map.keys
    |> Set.ofSeq

let evalNumberOfCatsPresent (configuration: Configuration) n =
    configuration
    |> Map.values
    |> Seq.toList
    |> List.filter (fun c -> c <> NoCat)
    |> fun cats -> List.length cats = n
    
let evalCatsPresent (configuration: Configuration) cats =
    configuration
    |> Map.values
    |> Seq.toList
    |> List.filter (fun c -> c <> NoCat)
    |> Set.ofList
    |> fun presentCats -> presentCats = cats

let evalInFront (configuration: Configuration) cats places =
    places
    |> Set.toList
    |> List.exists (fun place -> Set.contains (Map.find place configuration) cats)


let checkSinglePlace (fn: Place -> Place) (configuration: Configuration) cats catsOrPlaces  =
    match catsOrPlaces with
    | Cats(targetCats) -> 
        catPlaces configuration cats
        |> Set.map (fn >> catAtPlace configuration)
        |> Set.intersect targetCats 
        |> Set.isEmpty |> not
    | Places(places) -> 
        catPlaces configuration cats
        |> Set.map fn
        |> Set.intersect places
        |> Set.isEmpty |> not

let evalAcrossFrom = checkSinglePlace placeAcrossFrom

let evalPlacesFrom (configuration: Configuration) cats n catsOrPlaces =
    match catsOrPlaces with
    | Cats(targetCats) -> 
        catPlaces configuration cats
        |> Set.toList
        |> List.collect (placesAway n)
        |> Set.ofList
        |> Set.map (catAtPlace configuration)
        |> Set.intersect targetCats
        |> Set.isEmpty |> not
    | Places(places) -> 
        catPlaces configuration cats
        |> Set.toList
        |> List.collect (placesAway n)
        |> Set.ofList
        |> Set.intersect places
        |> Set.isEmpty |> not

let evalPlacesLeftOf (configuration: Configuration) cats n catsOrPlaces =
    checkSinglePlace (placesRight n) configuration cats catsOrPlaces
let evalPlacesRightOf (configuration: Configuration) cats n catsOrPlaces =
    checkSinglePlace (placesLeft n) configuration cats catsOrPlaces

let evalFact (configuration: Configuration) fact = 
    match fact with
    | NumberOfCatsPresent(n) -> evalNumberOfCatsPresent configuration n
    | CatsPresent(cats) -> evalCatsPresent configuration cats
    | InFront(cat, places) ->  evalInFront configuration cat places
    | AcrossFrom(cats, catsOrPlaces) -> evalAcrossFrom configuration cats catsOrPlaces
    | PlacesFrom(cats, n, catsOrPlaces) -> evalPlacesFrom configuration cats n catsOrPlaces
    | PlacesLeftOf(cats, n, catsOrPlaces) -> evalPlacesLeftOf configuration cats n catsOrPlaces
    | PlacesRightOf(cats, n, catsOrPlaces) -> evalPlacesRightOf configuration cats n catsOrPlaces

let rec evalLogic configuration = function
    | Fact(fact) -> evalFact configuration fact
    | Not(l) -> not (evalLogic configuration l)
    | And(ls) -> 
        ls
        |> List.map (evalLogic configuration)
        |> List.fold (&&) true
    | Or(ls) ->
        ls
        |> List.map (evalLogic configuration)
        |> List.fold (||) false

let catInFrontOfPlace cat place = Fact(InFront(set [cat], set [place]))

let catInFrontOfItem cat item = Fact(InFront(set [cat], placesWith(item)))

let catAcrossFromCat cat cat2 = Fact(AcrossFrom(set [cat], Cats(set [cat2])))

let catAcrossFromPlace cat place = Fact(AcrossFrom(set [cat], Places(set place)))

let catPlacesFromCat cat n cat2 = Fact(PlacesFrom(set [cat], n, Cats(set [cat2])))

let catPlacesFromPlace cat n place = Fact(PlacesFrom(set [cat], n, Places(set [place])))

let allCatsPresent = Fact(CatsPresent(set [MrMittens; Sassy; Ginger; TomCat; PipSqueak; Duchess]))

let between cats catsOrPlacesA catsOrPlacesB = Or(
    [ And(
        [ Fact(PlacesLeftOf(cats, 1, catsOrPlacesA))
        ; Fact(PlacesRightOf(cats, 1, catsOrPlacesB))
        ]
    )
    ; And(
        [ Fact(PlacesRightOf(cats, 1, catsOrPlacesA))
        ; Fact(PlacesLeftOf(cats, 1, catsOrPlacesB))
        ]
    )
    ]
)

let exclusiveOr a b = And(
    [ Or([a; b])
    ; Not(And([a; b]))
    ]
)


let allConfigs () = 
    let allCats = set [NoCat; MrMittens; Ginger; PipSqueak; TomCat; Sassy; Duchess]
    seq {
        for c1 in allCats do
        for c2 in Set.difference allCats (set [c1]) |> Set.add NoCat do
        for c3 in Set.difference allCats (set [c1; c2]) |> Set.add NoCat do
        for c4 in Set.difference allCats (set [c1; c2; c3]) |> Set.add NoCat do
        for c5 in Set.difference allCats (set [c1; c2; c3; c4]) |> Set.add NoCat do
        for c6 in Set.difference allCats (set [c1; c2; c3; c4; c5]) |> Set.add NoCat do
            yield Map.ofList 
                [ Birdcage, c1
                ; Coffee, c2
                ; Shoes, c3
                ; Fishbowl, c4
                ; Yarn, c5
                ; Plant, c6
                ]
    }

let solve logic =
    allConfigs ()
    |> Seq.filter (fun config -> evalLogic config logic)
    |> Seq.toList


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

let problem4: Logic = And(
    [ Fact(NumberOfCatsPresent(3))
    ; catAcrossFromCat Ginger Sassy
    ; catPlacesFromCat Sassy 1 MrMittens
    ; catPlacesFromPlace Ginger 1 Birdcage
    ; Fact(PlacesRightOf(set [Ginger], 1, Cats(set [MrMittens])))
    ]
)

let testConfig: Configuration =
    [ Birdcage, Ginger
    ; Coffee, Duchess
    ; Shoes, TomCat
    ; Fishbowl, MrMittens
    ; Yarn, Sassy
    ; Plant, PipSqueak
    ]
    |> Map.ofList

let test () = evalLogic testConfig problem7