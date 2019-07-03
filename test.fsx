#load @"c:\Users\nagla\projects\f#\testrunner\lib\types\DataParser.Types.fs"
#load @"c:\Users\nagla\projects\f#\testrunner\lib\DataParser.fs"
#load @"c:\Users\nagla\projects\f#\testrunner\lib\types\Entity.Types.fs"
#load @"c:\Users\nagla\projects\f#\testrunner\lib\Entity.fs"
#load @"c:\Users\nagla\projects\f#\testrunner\lib\types\TestCase.Types.fs"
#load @"c:\Users\nagla\projects\f#\testrunner\lib\TestCase.fs"


open System.IO
open TestRunner.DataParser
open TestRunner.TestCase

let lines = File.ReadAllLines ("test.txt") |> List.ofSeq

let testCase = parse lines |> createTestCase

let lines' = testCase |> revertTestcase |> toFile
File.WriteAllLines("test2.txt", lines')