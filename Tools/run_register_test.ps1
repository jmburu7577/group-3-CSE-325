$base='http://localhost:5019'
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
$r = Invoke-WebRequest -Uri ($base + '/Account/Register') -WebSession $session -UseBasicParsing -ErrorAction Stop
$html = $r.Content
$m = [regex]::Match($html,'name="__RequestVerificationToken"[^>]*value="([^"]+)"')
$token = $m.Groups[1].Value
$email = "test.user.$(Get-Date -UFormat %s)@example.test"
$form = @{
  '__RequestVerificationToken' = $token
  'Input.FirstName'='Test'
  'Input.LastName'='User'
  'Input.Email'=$email
  'Input.Password'='P@ssw0rd123!'
  'Input.ConfirmPassword'='P@ssw0rd123!'
  'Input.DateOfBirth' = (Get-Date).AddYears(-30).ToString('yyyy-MM-dd')
  'Input.PhoneNumber'='555-0100'
  'Input.Address'='123 Test St'
}
try {
  $resp = Invoke-WebRequest -Uri ($base + '/Account/Register') -Method Post -Body $form -WebSession $session -ContentType 'application/x-www-form-urlencoded' -MaximumRedirection 0 -ErrorAction Stop
  if ($resp.StatusCode -eq 302) { Write-Output "REGISTERED_REDIRECT:$email -> $($resp.Headers.Location)" } else { Write-Output "REGISTER_RESPONSE_STATUS: $($resp.StatusCode)" }
} catch [System.Net.WebException] {
  Write-Output "REGISTER_POST_EXCEPTION: $($_.Exception.Message)"
  if ($_.Exception.Response -ne $null) {
    $sr = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream()); $body = $sr.ReadToEnd(); Write-Output " RESPONSE_BODY:"; Write-Output $body
  }
}

$homeResp = Invoke-WebRequest -Uri $base -WebSession $session -UseBasicParsing -ErrorAction Stop
if ($homeResp.Content -match [regex]::Escape($email)) { Write-Output "SIGNED_IN: $email" } else { Write-Output 'SIGNED_IN_NOT_DETECTED'; if ($homeResp.Content -match 'Logout|Sign out|Sign Out') { Write-Output 'Logout link present (likely signed in)' } else { Write-Output 'No logout link detected' } }
