/**
 * @license Copyright (c) 2003-2015, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
    config.filebrowserImageUploadUrl = "Upload";

    config.toolbarGroups = [
		{ name: 'document', groups: ['document', 'doctools', 'mode'] },
		{ name: 'clipboard', groups: ['undo', 'clipboard'] },
		{ name: 'editing', groups: ['find', 'selection', 'spellchecker', 'editing'] },
		{ name: 'forms', groups: ['forms'] },
		{ name: 'insert', groups: ['insert'] },
		{ name: 'links', groups: ['links'] },
		'/',
		{ name: 'styles', groups: ['styles'] },
		{ name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
		{ name: 'colors', groups: ['colors'] },
		{ name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi', 'paragraph'] },
		'/',
		{ name: 'tools', groups: ['tools'] },
		{ name: 'others', groups: ['others'] },
		{ name: 'about', groups: ['about'] }
    ];

    config.removeButtons = 'About,Maximize,ShowBlocks,Iframe,PageBreak,Language';
};