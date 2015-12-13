/**
 * @license Copyright (c) 2003-2015, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
    config.toolbarGroups = [
        { name: 'clipboard', groups: ['undo', 'clipboard'] },
        { name: 'document', groups: ['document', 'doctools', 'mode'] },
        { name: 'forms', groups: ['forms'] },
        { name: 'insert', groups: ['insert'] },
        { name: 'editing', groups: ['find', 'selection', 'spellchecker', 'editing'] },
        { name: 'links', groups: ['links'] },
        { name: 'about', groups: ['about'] },
        '/',
        { name: 'styles', groups: ['styles'] },
        { name: 'colors', groups: ['colors'] },
        { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
        { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi', 'paragraph'] },
        '/',
        { name: 'tools', groups: ['tools'] },
        { name: 'others', groups: ['others'] }
    ];

    config.removeButtons = 'Maximize,ShowBlocks,Iframe,PageBreak,Language,ImageButton,HiddenField';


    config.filebrowserImageUploadUrl = "/Home/Upload";
};