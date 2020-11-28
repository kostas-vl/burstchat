import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SidebarSelectionComponent } from './sidebar-selection.component';

describe('SidebarSelectionComponent', () => {
    let component: SidebarSelectionComponent;
    let fixture: ComponentFixture<SidebarSelectionComponent>;

    beforeEach(waitForAsync(() => {
        TestBed.configureTestingModule({
            declarations: [SidebarSelectionComponent]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(SidebarSelectionComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
