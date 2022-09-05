@echo off
cls
setlocal enableextensions



set input1_01=.\hcs_wsdl_xsd_v.11.0.10.2\bills\*.wsdl
set input1_02=.\hcs_wsdl_xsd_v.11.0.10.2\capital-repair\*.wsdl
set input1_03=.\hcs_wsdl_xsd_v.11.0.10.2\device-metering\*.wsdl
set input1_04=.\hcs_wsdl_xsd_v.11.0.10.2\fas\*.wsdl
set input1_05=.\hcs_wsdl_xsd_v.11.0.10.2\house-management\*.wsdl
set input1_06=.\hcs_wsdl_xsd_v.11.0.10.2\infrastructure\*.wsdl
set input1_07=.\hcs_wsdl_xsd_v.11.0.10.2\inspection\*.wsdl
rem set input1_08=.\hcs_wsdl_xsd_v.11.0.10.2\lib\*.wsdl
set input1_09=.\hcs_wsdl_xsd_v.11.0.10.2\licenses\*.wsdl
set input1_10=.\hcs_wsdl_xsd_v.11.0.10.2\nsi\*.wsdl
set input1_11=.\hcs_wsdl_xsd_v.11.0.10.2\nsi-common\*.wsdl
set input1_12=.\hcs_wsdl_xsd_v.11.0.10.2\organizations-registry\*.wsdl
set input1_13=.\hcs_wsdl_xsd_v.11.0.10.2\organizations-registry-common\*.wsdl
set input1_14=.\hcs_wsdl_xsd_v.11.0.10.2\payment\*.wsdl
set input1_15=.\hcs_wsdl_xsd_v.11.0.10.2\services\*.wsdl

set input2_01=.\hcs_wsdl_xsd_v.11.0.10.2\bills\*.xsd
set input2_02=.\hcs_wsdl_xsd_v.11.0.10.2\capital-repair\*.xsd
set input2_03=.\hcs_wsdl_xsd_v.11.0.10.2\device-metering\*.xsd
set input2_04=.\hcs_wsdl_xsd_v.11.0.10.2\fas\*.xsd
set input2_05=.\hcs_wsdl_xsd_v.11.0.10.2\house-management\*.xsd
set input2_06=.\hcs_wsdl_xsd_v.11.0.10.2\infrastructure\*.xsd
set input2_07=.\hcs_wsdl_xsd_v.11.0.10.2\inspection\*.xsd
set input2_08=.\hcs_wsdl_xsd_v.11.0.10.2\lib\*.xsd
set input2_09=.\hcs_wsdl_xsd_v.11.0.10.2\licenses\*.xsd
set input2_10=.\hcs_wsdl_xsd_v.11.0.10.2\nsi\*.xsd
set input2_11=.\hcs_wsdl_xsd_v.11.0.10.2\nsi-common\*.xsd
set input2_12=.\hcs_wsdl_xsd_v.11.0.10.2\organizations-registry\*.xsd
set input2_13=.\hcs_wsdl_xsd_v.11.0.10.2\organizations-registry-common\*.xsd
set input2_14=.\hcs_wsdl_xsd_v.11.0.10.2\payment\*.xsd
set input2_15=.\hcs_wsdl_xsd_v.11.0.10.2\services\*.xsd


set output=/out:.\GisBustedWsdlWrapper 
set output_config=/config:GisBustedWsdlWrapper
set output_namespace=/namespace:*,GisBustedWsdlWrapper
set params= /targetClientVersion:Version35 /async /language:C#  

"C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools\svcutil.exe" %input1_01% %input1_02% %input1_03% %input1_04% %input1_05% %input1_06% %input1_07%             %input1_09% %input1_10% %input1_11% %input1_12% %input1_13% %input1_14% %input1_15%     %input2_01% %input2_02% %input2_03% %input2_04% %input2_05% %input2_06% %input2_07% %input2_08% %input2_09% %input2_10% %input2_11% %input2_12% %input2_13% %input2_14% %input2_15%     %output% %output_namespace% %output_config% %params%

del GisBustedWsdlWrapper.config


