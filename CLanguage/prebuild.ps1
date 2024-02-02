$oldPWD = Get-Location
Set-Location Parser
$skeleton = Get-Content -Raw "../../Lib/skeleton.cs"
$skeleton | &"../../Lib/jay.exe" -vc CParser.jay > CParser.cs
Set-Location $oldPWD