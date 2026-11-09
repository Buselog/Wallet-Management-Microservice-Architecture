import { useRef, useState } from 'react';
import { UploadSimple, File, FileText, X, Image as ImageIcon } from '@phosphor-icons/react';
import { cn } from '../../lib/utils';

const ACCEPT = '.pdf,.png,.jpg,.jpeg';

/**
 * Drag-and-drop file picker with a click fallback (WCAG 2.2 dragging movements).
 * Validation is delegated to the parent via onSelectFile(file) which returns boolean.
 */
export default function ReceiptDropzone({ file, onSelectFile, onClear, error, loading }) {
  const inputRef = useRef(null);
  const [isDragging, setIsDragging] = useState(false);

  const openPicker = () => {
    if (loading) return;
    inputRef.current?.click();
  };

  const handleFiles = (files) => {
    if (!files || files.length === 0) return;
    onSelectFile?.(files[0]);
  };

  const handleDrop = (event) => {
    event.preventDefault();
    setIsDragging(false);
    handleFiles(event.dataTransfer?.files);
  };

  const isPdf = file?.name?.toLowerCase().endsWith('.pdf');
  const isImage = file && !isPdf;

  return (
    <div className="flex flex-col gap-3">
      <button
        type="button"
        onClick={openPicker}
        onDragOver={(event) => {
          event.preventDefault();
          if (!loading) setIsDragging(true);
        }}
        onDragLeave={() => setIsDragging(false)}
        onDrop={handleDrop}
        disabled={loading}
        aria-label="Upload receipt or invoice file"
        className={cn(
          'group relative flex min-h-[220px] w-full flex-col items-center justify-center gap-3 rounded-2xl border-2 border-dashed p-6 text-center transition-colors',
          'focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2',
          isDragging ? 'border-primary bg-primary/5' : 'border-border bg-muted/30 hover:border-primary/60 hover:bg-muted/50',
          loading && 'cursor-not-allowed opacity-60',
        )}
      >
        <input
          ref={inputRef}
          type="file"
          accept={ACCEPT}
          className="hidden"
          onChange={(event) => handleFiles(event.target.files)}
          disabled={loading}
        />

        {!file && (
          <>
            <div className="flex h-14 w-14 items-center justify-center rounded-2xl bg-background text-primary shadow-sm">
              <UploadSimple size={28} weight="regular" aria-hidden="true" />
            </div>
            <div>
              <p className="text-sm font-semibold text-foreground">
                Drag & drop your receipt or invoice here
              </p>
              <p className="mt-1 text-xs text-muted-foreground">
                or click to browse — PDF, PNG, JPG (max 5 MB)
              </p>
            </div>
          </>
        )}

        {file && (
          <div className="flex w-full items-center gap-4 rounded-xl bg-background p-4 shadow-sm">
            <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-muted text-muted-foreground">
              {isPdf ? (
                <File size={24} weight="regular" aria-hidden="true" />
              ) : isImage ? (
                <ImageIcon size={24} weight="regular" aria-hidden="true" />
              ) : (
                <FileText size={24} weight="regular" aria-hidden="true" />
              )}
            </div>
            <div className="min-w-0 flex-1 text-left">
              <p className="truncate text-sm font-semibold text-foreground">{file.name}</p>
              <p className="text-xs text-muted-foreground">
                {(file.size / 1024).toFixed(1)} KB
              </p>
            </div>
            <button
              type="button"
              onClick={(event) => {
                event.stopPropagation();
                onClear?.();
              }}
              disabled={loading}
              aria-label="Remove selected file"
              className="flex h-8 w-8 items-center justify-center rounded-lg text-muted-foreground transition-colors hover:bg-muted hover:text-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
            >
              <X size={16} weight="bold" aria-hidden="true" />
            </button>
          </div>
        )}
      </button>

      {error && (
        <p role="alert" className="text-sm font-medium text-destructive">
          {error}
        </p>
      )}
    </div>
  );
}
