localhost/:1 Access to fetch at 'http://localhost:8081/api/files/index-local' from origin 'http://localhost:5173' has been blocked by CORS policy: No 'Access-Control-Allow-Origin' header is present on the requested resource. If an opaque response serves your needs, set the request's mode to 'no-cors' to fetch the resource with CORS disabled.
pdfStore.ts?t=1771108348180:14 
        
        
       POST http://localhost:8081/api/files/index-local net::ERR_FAILED 500 (Internal Server Error)
selectPdf @ pdfStore.ts?t=1771108348180:14
wrappedAction @ pinia.js?v=91e36ce6:5507
store.<computed> @ pinia.js?v=91e36ce6:5204
confirmChoose @ ChooseFile.vue?t=1771108893859:107
callWithErrorHandling @ chunk-C2CMTVXR.js?v=91e36ce6:2350
callWithAsyncErrorHandling @ chunk-C2CMTVXR.js?v=91e36ce6:2357
invoker @ chunk-C2CMTVXR.js?v=91e36ce6:11454
Show 5 more frames
Show less

clear table
`sqlite3 backend/storage/pdf_index.db "DELETE FROM pdf_files; DELETE FROM sqlite_sequence WHERE name='pdf_files'; VACUUM;"`