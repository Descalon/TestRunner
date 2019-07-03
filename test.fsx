#load @".\lib\types\DataParser.Types.fs"
#load @".\lib\DataParser.fs"
#load @".\lib\types\Entity.Types.fs"
#load @".\lib\Entity.fs"
#load @".\lib\types\TestCase.Types.fs"
#load @".\lib\TestCase.fs"


open System.IO
open TestRunner.DataParser
open TestRunner.TestCase

let lines = File.ReadAllLines ("test.txt") |> List.ofSeq

let testCase = parse lines |> createTestCase

let lines' = testCase |> revertTestcase |> toFile
File.WriteAllLines("test2.txt", lines')