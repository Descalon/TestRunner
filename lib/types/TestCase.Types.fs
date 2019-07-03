namespace TestRunner.TestCase
open TestRunner.Entity.Types
open TestRunner.DataParser.Types

module Types =

    type TestCaseHeader = {
        CaseId: RecordData
        ClassName: RecordData
        MethodName: RecordData
        EntityType: RecordData
    }

    let defaultHeader = {
        CaseId = ("", defaultRecordLine)
        ClassName = ("", defaultRecordLine)
        MethodName = ("", defaultRecordLine)
        EntityType = ("", defaultRecordLine)
    }

    type TestCase = {
        Header: TestCaseHeader
        Id: string
        MainEntity: Entity
        Entities: Entity list
    }