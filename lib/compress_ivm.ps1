Param(
$infile="",
$outfile=""
);

$c = Get-Content $infile -Encoding UTF8 

$r = [string]::Join("
", $c)

$strBytes=[System.Text.Encoding]::UTF8.GetBytes($c)
$to64 = [System.Convert]::ToBase64String($strBytes)
$ms = New-Object System.IO.MemoryStream
$cs = New-Object System.IO.Compression.GZipStream($ms, [System.IO.Compression.CompressionMode]::Compress)

$sw = New-Object System.IO.StreamWriter($cs)
$sw.Write($r)
$sw.Close();

$bytes = $ms.ToArray()
$compress=[System.Convert]::ToBase64String($bytes)

Write-host "Original size:" $c.Length
Write-host "Base64 size: " $to64.Length
Write-host "Compressed size: " $compress.Length
$compress
$compress | Out-File -FilePath $outfile -Force

$y=Get-Content $outfile
$y
$data = [System.Convert]::FromBase64String($y)

$ms = New-Object System.IO.MemoryStream
$ms.Write($data, 0, $data.Length)
$ms.Seek(0,0) | Out-Null

$cs = New-Object System.IO.Compression.GZipStream($ms, [System.IO.Compression.CompressionMode]::Decompress)
$sr = New-Object System.IO.StreamReader($cs)
$t = $sr.readtoend()
Write-Host "Finally: " $t
