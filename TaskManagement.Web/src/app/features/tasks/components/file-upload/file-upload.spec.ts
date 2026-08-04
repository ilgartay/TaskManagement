import { TestBed } from '@angular/core/testing';
import { FileUpload } from './file-upload';

describe('FileUpload', () => {
  beforeEach(() => TestBed.configureTestingModule({ imports: [FileUpload] }));

  it('emits files up to ten megabytes', () => {
    const fixture = TestBed.createComponent(FileUpload);
    const selected: File[] = [];
    fixture.componentInstance.selected.subscribe((file) => selected.push(file));
    const file = new File(['content'], 'notes.txt', { type: 'text/plain' });

    fixture.componentInstance.choose(fileEvent(file));

    expect(selected).toEqual([file]);
    expect(fixture.componentInstance.error()).toBe('');
  });

  it('rejects files larger than ten megabytes', () => {
    const fixture = TestBed.createComponent(FileUpload);
    const file = new File(['content'], 'large.zip');
    Object.defineProperty(file, 'size', { value: 11 * 1024 * 1024 });

    fixture.componentInstance.choose(fileEvent(file));

    expect(fixture.componentInstance.error()).toContain('10 MB');
  });
});

function fileEvent(file: File): Event {
  return { target: { files: [file], value: 'selected' } } as unknown as Event;
}
