namespace TestRunner

open System
open TestRunner.DataParser.Types
open System.Text.RegularExpressions


module DataParser =
    let private parseCodeLine (line:string) (rLine: RecordLine)=
        let code = (int) line.[..1]
        let id = line.[2..]
        {rLine with 
            Code = code
            Id = Some id
        }
    
    let private parseDateLine (line:string) (rline: RecordLine) = {
        rline with 
            Date = Some line
    }

    let private  parseKey (line:string) (rline: RecordLine) = {
        rline with 
            Key = Some line
    }

    let private  parseIOCodeValue (line:string) (rline: RecordLine) = 
        let ioCode = line.[..2]
        let value = line.[3..]
        {
            rline with 
                IOCode = Some ioCode
                Value = Some value
        }
        

    let private parseSingleLine (line:string) = 
        let split = line.Split([|';'|], StringSplitOptions.RemoveEmptyEntries)
        let colSize = split.Length
        let hasDate = colSize > 3
        let rec fn index rline =
            if index >= colSize then rline else
                let col = split.[index]
                match index with
                | 0 -> 
                    let rline' = parseCodeLine col rline
                    fn (index + 1) rline'
                | 1 -> 
                    let mutable index' = index
                    let mutable rline' = rline
                    if hasDate then 
                        rline' <- parseDateLine col rline
                        index' <- (index + 1)

                    let col' = split.[index']
                    let rline'' = parseKey col' rline'
                    fn (index + 1) rline''
                | 2 | 3 ->
                    let rline' = parseIOCodeValue col rline
                    fn (index + 1) rline'
                | _ -> rline
        fn 0 defaultRecordLine
        

    let private parseFile = List.map (parseSingleLine)
    let private cleanLine input = Regex.Replace (input, "\s+", ";")

    let parse = 
        List.map cleanLine >> parseFile
    
    let private padRight width (line:string) = 
        line.PadRight(width)

    let strip = function
    | Some x -> x
    | None -> ""

    let toSingleLine (line:RecordLine) = 
        let file = (line.Code |> string) |> padRight 2
        let map = [
            (strip line.Id, columnPadding.IdPadding);
            (strip line.Date, columnPadding.DatePadding);
            (strip line.Key, columnPadding.KeyPadding);
            (strip line.IOCode, columnPadding.IOCodePadding);
            (strip line.Value, columnPadding.ValuePadding);
        ] 
        
        let folder (state:string) (v:string, width:int) =
            (state |> padRight width) + v

        map
        |> List.fold folder file
        |> padRight 203
    
    let toFile = List.map toSingleLine
            
            