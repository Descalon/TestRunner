namespace TestRunner.DataParser

module Types =
    type RecordLine = {
        Code : int
        Id : string option
        Key : string option
        Date: string option
        IOCode: string option
        Value: string option
    }
    type RecordData = string * RecordLine

    type IVerified = 
        abstract member RecordLine: RecordLine

    type ColumnPadding = {
        CodePadding : int
        IdPadding : int
        KeyPadding : int
        DatePadding : int
        IOCodePadding : int
        ValuePadding : int
    }

    let columnPadding = {
        CodePadding = 0
        IdPadding  = 2
        DatePadding = 32
        KeyPadding = 52
        IOCodePadding = 162
        ValuePadding = 165
    }

    let defaultRecordLine : RecordLine = {
        Code = 00
        Id = Some ""
        Key = Some ""
        Date = Some ""
        IOCode = Some ""
        Value = Some ""
    }

    type DataFile = RecordLine list