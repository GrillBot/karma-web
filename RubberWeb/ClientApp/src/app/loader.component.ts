import { Component } from '@angular/core';

@Component({
    selector: 'app-loader',
    template: '<img src="/assets/duck.gif" width="256" height="256" /> <h1>Načítání...</h1>',
    styles: [
        'img { display: block; }',
        'h1 { text-align: center; font-weight: 100; color: #99AAB5 }'
    ]
})
export class LoaderComponent { }
