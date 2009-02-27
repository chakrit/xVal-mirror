﻿
/* 
 * Nota: En este momento estos mensajes solo funcionan si utiliza valdacion con JQuery (no con validacion nativa a ASP.NET)
 * Soporte para validacion con ASP.NET sera anidida pronto.
*/
var xVal = xVal || {};
xVal.Messages = {
    "Required" : "Este valor es necesario.",
    "DataType_EmailAddress" : "Por favor ingrese una direccion de email valida.",
    "DataType_Integer" : "Por favor ingrese un numero entero.",
    "DataType_Decimal" : "Por favor ingrese un valor numerico.",
    "DataType_Date" : "Por favor ingrese una fecha valida.",
    "DataType_DateTime" : "Por favor ingrese fecha y hora.",
    "DataType_Currency" : "Por favor ingrese una cantidad de dinero.",
    "DataType_CreditCardLuhn" : "Por favor ingrese un numero de tarjeta de credito valido.",
    "Regex" : "Este valor es invalido.",
    "Range_Numeric_Min" : "Por favor ingrese un numero mayor o igual a {0}.",
    "Range_Numeric_Max" : "Por favor ingrese un numero menor o igual a {0}.",
    "Range_Numeric_MinMax" : "Por favor ingrese un numero entre {0} y {1}.",
    "Range_String_Min" : "Por favor ingrese un valor no alfabetico antes de '{0}'.",
    "Range_String_Max" : "Por favor ingrese un valor no alfabetico antes de '{0}'.",
    "Range_String_MinMax" : "Por favor ingrese un texto entre '{0}' y '{1}'.",
    "Range_DateTime_Min" : "Por favor ingrese una fecha posterior a {0}.",
    "Range_DateTime_Max": "Por favor ingrese una fecha anterior a {0}.",
    "Range_DateTime_MinMax": "Por favor ingrese una fecha entre {0} y {1}.",
    "StringLength_Min": "Por favor ingrese un minimo de {0} caracteres.",
    "StringLength_Max": "Por favor no ingrese mas de {0} caracteres.",
    "StringLength_MinMax": "Por favor ingrese un texto entre {0} y {1} caracteres.",
    "Comparison_Equals" : "Este valor tiene wue ser igual a {0}.",
    "Comparison_DoesNotEqual" : "Este valor tiene que ser diferente que {0}."
};