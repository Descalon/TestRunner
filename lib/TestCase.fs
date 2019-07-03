namespace TestRunner

open System.Collections.Generic
open DataParser.Types
open TestCase.Types
open Entity
open System.Collections

module TestCase =
    type private RecordMap = IDictionary<int,RecordLine>

    let private validate = function
    | Some x -> x 
    | None -> failwith "All TestCase properties should have a value"

    let private validateRecordData (x,y) = (validate x, y) 

    let private toDict datafile = 
        datafile
        |> List.map (fun x -> x.Code, x)
        |> dict

    let private getItem (map:RecordMap) index = map.Item(index)
    let private getKey (map:RecordMap) index = 
        let item = (getItem map index)
        (item.Key, item)

    let CreateHeader dataFile =
        let map = dataFile |> toDict
        let key = getKey map >> validateRecordData
        let item i = map.Item(i)
        let caseItem = item 42
        {
            CaseId = validateRecordData <| (caseItem.Id, caseItem)
            ClassName = key 35
            MethodName = key 36
            EntityType = key 43
        }

    let CreateEntities dataFile = 
        let predicate = fun x -> x.Code <> 44
        let skip = List.skipWhile predicate
        let take = List.takeWhile predicate

        let n = skip dataFile
        let rec fn entities = function
        | [] -> entities
        | (x::xs) ->
            let e' = [x] @ take xs
            let rest = skip xs
            fn ([buildEntity e'] @ entities) rest

        fn [] n

    let apply f x = f x

    let getId dataFile =
        dataFile
        |> List.filter (fun x -> x.Code = 42)
        |> List.head
        |> apply (fun x -> x.Id)
        |> validate

    let createTestCase datafile = 
        let entities = CreateEntities datafile |> List.rev
        {
            Header = CreateHeader datafile
            Id = getId datafile
            MainEntity = entities.[0]
            Entities = entities.[1..]
        } 
    let getRecord (_,y) = y

    let filler = [
        {defaultRecordLine with Code = 40; Id = Some "SetName"};
        {defaultRecordLine with Code = 41}
    ]

    let revertHeader header =
        [getRecord header.ClassName] @
        [getRecord header.MethodName] @
        filler @
        [getRecord header.CaseId] @
        [getRecord header.EntityType]

    let revertTestcase testCase = 
        let lines = [defaultRecordLine] @
                    (revertHeader testCase.Header) @
                    (revertEntity testCase.MainEntity)

        let l = List.map (revertEntity) >> List.fold (@) []

        lines @ (l testCase.Entities)