import { TestBed, waitForAsync } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { RootComponent } from './root.component';

describe('RootComponent', () => {
    beforeEach(waitForAsync(() => {
        TestBed.configureTestingModule({
            imports: [
                RouterTestingModule
            ],
            declarations: [
                RootComponent
            ],
        }).compileComponents();
    }));

    it('should create the app', () => {
        const fixture = TestBed.createComponent(RootComponent);
        const app = fixture.debugElement.componentInstance;
        expect(app).toBeTruthy();
    });

    it(`should have as title 'burst-chat-client'`, () => {
        const fixture = TestBed.createComponent(RootComponent);
        const app = fixture.debugElement.componentInstance;
        expect(app.title).toEqual('burst-chat-client');
    });

    it('should render title in a h1 tag', () => {
        const fixture = TestBed.createComponent(RootComponent);
        fixture.detectChanges();
        const compiled = fixture.debugElement.nativeElement;
        expect(compiled.querySelector('h1').textContent).toContain('Welcome to burst-chat-client!');
    });
});
