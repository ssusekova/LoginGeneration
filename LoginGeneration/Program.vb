Imports System
Imports System.IO
Imports System.Security.Cryptography.X509Certificates
Imports System.Text.RegularExpressions

Module Program
    Public Dict As New Dictionary(Of String, Int32) ' словарь для хранения количества использований логинов
    Public Logins As New List(Of String) ' список сгенерированных логинов

    Sub Main(args As String())
        Dim FIOList As New List(Of String)
        Try
            FIOList = File.ReadAllLines("./Persons.txt").ToList()
        Catch ex As Exception
            Console.WriteLine("Exception: " + ex.Message)
        End Try

        For Each FIO In FIOList
            Dim Person = FIO.Split(" ")
            Dim Login As String = ""

            ' вариант <имя>.<фамилия>
            Login = Person(1).ToLower() + "." + Person(0).ToLower()
            AddToDictionary(ModifyLength(Login))

            If IsNext(Login) Then
                Logins.Add(Login)
                Continue For
            End If

            ' вариант <первая буква имени>.<фамилия>
            Login = Person(1).Substring(0, 1).ToLower() + "." + Person(0).ToLower()
            AddToDictionary(ModifyLength(Login))

            If IsNext(Login) Then
                Logins.Add(Login)
                Continue For
            End If

            ' вариант <первая буква имени>.<первая буква отчества>.<фамилия>
            Login = Person(1).Substring(0, 1).ToLower() + "." + Person(2).Substring(0, 1).ToLower() + "." + Person(0).ToLower()
            AddToDictionary(ModifyLength(Login))

            If IsNext(Login) Then
                Logins.Add(Login)
                Continue For
            End If

            ' вариант <фамилия>
            Login = Person(0).ToLower()
            AddToDictionary(ModifyLength(Login))

            If IsNext(Login) Then
                Logins.Add(Login)
                Continue For
            End If

            ' вариант <первая буква имени>.<фамилия><инкремент>
            Dim BaseLogin = Person(1).Substring(0, 1).ToLower() + "." + Person(0).ToLower()

            If Logins.Contains(BaseLogin) Then
                Dim LastDupLogin = Logins.Where(Function(value) value.StartsWith(BaseLogin) = True).LastOrDefault()
                Dim AmountOfDuplicates = Logins.Where(Function(x) x.StartsWith(BaseLogin)).Count
                Dim DigitString = Regex.Replace(LastDupLogin, "[^0-9]", "")
                Dim Digit As Integer
                Dim IsExistsNumberInLogin = Int32.TryParse(DigitString, Digit)
                Login = If(IsExistsNumberInLogin = True, BaseLogin + (Digit + 1).ToString(), BaseLogin + AmountOfDuplicates.ToString())
            Else
                Login = BaseLogin
            End If
            AddToDictionary(ModifyLength(Login))
            If IsNext(Login) Then
                Logins.Add(Login)
                Continue For
            End If

        Next

        Using writter As New StreamWriter("./Logins.txt", False)

            For Each line In Logins
                writter.WriteLine(line)

            Next

        End Using

        Console.WriteLine("Сгенерированные логини записаны в файл Logins.txt в папке дебага")

    End Sub

    Sub AddToDictionary(login As String)
        Dim count = Dict.GetValueOrDefault(login, 0)
        count += 1
        Dict(login) = count
    End Sub

    Public Function ModifyLength(login As String) As String
        If login.Length > 20 Then
            Return login.Substring(0, 20)
        End If
        Return login
    End Function

    Public Function IsNext(login As String) As Boolean
        Dim count As Int32 = Dict.GetValueOrDefault(login, 0)
        Return count = 1
    End Function

End Module
