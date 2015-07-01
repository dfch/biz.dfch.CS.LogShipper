PARAM
(
	[Parameter(Mandatory = $true, Position = 0)]
	[Alias("line")]
	[String] $InputObject
)
$datNow = [datetime]::Now.ToString('yyyy-MM-dd HH:mm:ss.fffzzz');
$al = New-Object System.Collections.ArrayList;
$null = $al.Add(0);

$msg = "{0} - somescript.ps1 - '{1}'" -f $datNow, $InputObject;
[System.Diagnostics.Trace]::WriteLine($msg);
$null = $al.Add($msg);

# $ht = @{};
# $ht.msg = $InputObject;
# $null = $InputObject -match "(\w+)";

return @( $al );

<##
 #
 #
 # Copyright 2015 Ronald Rink, d-fens GmbH
 #
 # Licensed under the Apache License, Version 2.0 (the "License");
 # you may not use this file except in compliance with the License.
 # You may obtain a copy of the License at
 #
 # http://www.apache.org/licenses/LICENSE-2.0
 #
 # Unless required by applicable law or agreed to in writing, software
 # distributed under the License is distributed on an "AS IS" BASIS,
 # WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 # See the License for the specific language governing permissions and
 # limitations under the License.
 #
 #>
