namespace TestRunner

open DataParser.Types
open Entity.Types
open System

module Entity =
    let private parseAttributeIOType = function
    | 'I' -> Input
    | 'O' -> Output
    | 'B' -> IO
    | 'R' -> Result
    | _ -> AttributeIOType.Unknown

    let private parseAttributeType = function
    | 'R' -> Relation 
    | 'A' -> Attribute'
    | 'L' -> List 
    | _ -> AttributeType.Unknown

    let private validate = function
    | Some x -> x
    | None -> failwith "Entity properties should have values"

    let private buildAttribute (line:RecordLine) = {
        IOType = (validate line.IOCode).[0] |> parseAttributeIOType
        Type = (validate line.IOCode).[1] |> parseAttributeType
        Id = validate line.Id
        Key = validate line.Key
        Value = validate line.Value
        Date = line.Date
        RecordLine = line
    }

    let updateAttributeLineValue (attr:Entity.Types.Attribute) = 
        let line = attr.RecordLine
        let line' = {line with Value = Some attr.Value}
        {attr with RecordLine = line'}


    let private apply l f = f l
    let (<*>) = apply
    
    let buildEntity (lines: RecordLine list) =
        let attr = 
            lines 
            |> List.filter (fun x -> x.Code = 46)
            |> List.map buildAttribute

        let entityLine = 
            lines
            |> List.filter (fun x -> x.Code = 44)
            |> List.head
        let name = 
            entityLine <*> (fun x -> x.Key)
            |> validate

        let infoLine =
            lines |> List.filter (fun x -> x.Code = 45)
            |> List.head
        
        let info = infoLine <*> (fun x -> x.Id) |> validate
        {
            Name = name
            Info = (info, infoLine)
            Attributes = attr
            RecordLine = entityLine
        }
    
    let revertEntity (input:Entity) =
        let attributeLines = 
            input.Attributes
            |> List.map (fun x -> x.RecordLine)

        let (_, infoLine) = input.Info

        [input.RecordLine] @ [infoLine] @ attributeLines
            

