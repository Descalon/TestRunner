namespace TestRunner.Entity

open TestRunner.DataParser.Types

module Types =
    type AttributeIOType = Input | Output | IO | Result | Unknown
    type AttributeType = Relation | Attribute' | List | Unknown
    type Attribute = {
        Id: string
        Date: string option
        Key: string
        IOType: AttributeIOType
        Type: AttributeType
        Value: string
        RecordLine: RecordLine
    }
    type Entity = {
        Name : string
        Info: RecordData
        Attributes: Attribute list
        RecordLine: RecordLine
    }