# File Upload Architecture Implementation Guide

## Overview
Instead of using URLs (which have copyright issues), we'll implement local file storage in the `wwwroot/images` folder with a smart naming strategy.

## Naming Strategy: `product_{productId}_{imageType}_{timestamp}_{random}.{ext}`

### Examples:
- `product_8_main_20260218_a1b2c3d4.jpg`
- `product_8_mobile_20260218_b2c3d4e5.png`
- `product_8_gallery_20260218_c3d4e5f6.jpg`

### Benefits:
1. **Unique**: Timestamp + random prevents collisions
2. **Identifiable**: Can identify product and image type from filename
3. **Sortable**: Timestamp makes chronological ordering easy
4. **Safe**: Random suffix prevents enumeration attacks

## Architecture Components

### 1. Backend Storage
- **Folder**: `DotNetCoreWebApi/wwwroot/images/`
- **Structure**: 
  ```
  wwwroot/
  └── images/
      ├── product_8_main_20260218_a1b2c3d4.jpg
      ├── product_8_mobile_20260218_b2c3d4e5.jpg
      └── product_8_gallery_20260218_c3d4e5f6.jpg
  ```

### 2. Database Storage
- **ImageUrl field** stores: **relative path** (`images/product_8_main_20260218_a1b2c3d4.jpg`)
- **Benefits**: 
  - Portable across environments
  - Easy to serve as static files
  - Can migrate to different servers easily

### 3. API Endpoints

#### Upload Image File
```
POST /api/products/{productId}/images/upload
Content-Type: multipart/form-data

Parameters:
- file: File (binary)
- imageType: Integer (0=Main, 1=Mobile, 2=Gallery)
- displayOrder: Integer

Response:
{
  "id": 1,
  "idProduct": 8,
  "imageUrl": "images/product_8_main_20260218_a1b2c3d4.jpg",
  "imageType": 0,
  "displayOrder": 0,
  "uploadedDate": "2026-02-18T23:20:00Z"
}
```

#### Delete Image
```
DELETE /api/products/{productId}/images/{imageId}
Response: 204 No Content
```

### 4. Frontend Flow

#### Create Product with Images
1. User fills product form
2. User selects image files (not URLs)
3. Image previews show locally (using FileReader API)
4. User clicks "Create Product"
5. Frontend:
   - Creates product first
   - Uploads image files to `/upload` endpoint
   - Shows success notification
   - Navigates to products list

#### Edit Product
1. Images load from `imageUrl` (relative paths served from wwwroot)
2. User can add/remove images
3. New images upload immediately
4. Deleted images removed immediately

### 5. File Validation

**Frontend Validation:**
- File type: Only image/* accepted
- File size: Max 5MB (main), 4MB (mobile/gallery)
- Dimensions: Optional (can be validated backend)

**Backend Validation:**
- File type verification
- File size limits
- Physical file existence check
- Malware scanning (optional)

## Implementation Checklist

### Backend
- [ ] Create `wwwroot/images` directory
- [ ] Implement file upload endpoint in ProductImagesController
- [ ] Add file utility service for naming/saving
- [ ] Update ProductImageService to handle file uploads
- [ ] Add file cleanup when images deleted
- [ ] Configure static file serving in Program.cs

### Frontend
- [ ] Update ProductImageService with upload methods
- [ ] Update ProductImageUploadComponent to use file inputs
- [ ] Add FileReader for previews
- [ ] Add file validation logic
- [ ] Update create-vegproduct form structure
- [ ] Test upload flow end-to-end

## Security Considerations

1. **Filename Sanitization**: Use generated names, not original filenames
2. **Path Traversal**: Store files in wwwroot/images only
3. **File Type Verification**: Check MIME type and magic bytes
4. **Size Limits**: Enforce maximum file sizes
5. **Scanning**: Consider virus scanning for production
6. **CORS**: Configure for image serving
7. **Access Control**: Images served public (authenticated product)

## Performance Optimizations

1. **Image Compression**: Compress before serving
2. **CDN Ready**: Can move images to CDN easily
3. **Caching**: Set long-lived cache headers
4. **Lazy Loading**: Load images on demand
5. **Responsive Images**: Use `<picture>` element with srcset

## Migration Path

If migrating to cloud storage (Azure, AWS):
1. Update file service to upload to cloud
2. Update ImageUrl to use CDN URLs
3. Keep same database schema
4. No frontend changes needed
