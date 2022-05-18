#define WSQ_EXPORT __declspec( dllexport )


#if defined(__cplusplus)
extern "C" {
#endif

extern
WSQ_EXPORT int RegisterWSQ();


// Saves HBITMAP to an image file. Returns 1 if successfull, 0 otherwise 
extern
WSQ_EXPORT int SaveBMPToFile(HBITMAP hBitmap, const char *filename, int filetype);


// Creates an HBITMAP from an image file.  The extension of the file name
// determines the file type.  Returns an HBITMAP if successfull, NULL
// otherwise 
extern
WSQ_EXPORT HBITMAP CreateBMPFromFile(const char *filename);


// Creates an HBITMAP from WSQ compressed byte array.
// Returns an HBITMAP if successfull, NULL otherwise 
extern
WSQ_EXPORT HBITMAP CreateBMPFromWSQByteArray(unsigned char *input_wsq_byte_array, int size_of_input_wsq_byte_array);


// Saves WSQ compressed byte array to an image file.
// Returns 1 if successfull, 0 otherwise 
extern
WSQ_EXPORT int SaveWSQByteArrayToImageFile(unsigned char *input_wsq_byte_array, int size_of_input_wsq_byte_array, const char *filename, int filetype);


extern
WSQ_EXPORT int WSQ_decode_stream(unsigned char *input_data_stream, const int input_stream_length, unsigned char **output_data_stream, int *width, int *height, int *ppi, unsigned char **comment_text);


extern
WSQ_EXPORT int WSQ_encode_stream(unsigned char *input_data_stream, const int width, const int height, const double bitrate,
				   const int ppi, char *comment_text, unsigned char **output_data_stream, int *output_stream_length);


extern
WSQ_EXPORT void WriteWSQ_bitrate(double bitrate);

extern
WSQ_EXPORT double ReadWSQ_bitrate();

extern
WSQ_EXPORT void WriteWSQ_ppi(int ppi);

extern
WSQ_EXPORT int ReadWSQ_ppi();

extern
WSQ_EXPORT void WriteWSQ_comment(char *comment);

extern
WSQ_EXPORT char* ReadWSQ_comment();

extern
WSQ_EXPORT int ReadWSQ_implementation_number();

extern
WSQ_EXPORT int ReadTIFFcompression();


extern
WSQ_EXPORT void WriteTIFFcompression(int tiff_compression);


extern
WSQ_EXPORT int ReadTIFFpredictor();


extern
WSQ_EXPORT void WriteTIFFpredictor(int tiff_predictor);


extern
WSQ_EXPORT void ShowFileConverter();

extern
WSQ_EXPORT void SetShowFilePropertiesDialog(int file_properties_dialog);



extern
WSQ_EXPORT HBITMAP ConvertHBITMAPtoGrayScale256(HBITMAP hBitmap);

extern
WSQ_EXPORT int SaveHBITMAPtoFileAsGrayScale256BMP(HBITMAP hBitmap, const char *filename);



#if defined(__cplusplus)
 }
#endif
