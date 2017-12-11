; (function ($, window, document, undefined)
{
	'use strict';

	var pluginName = 'multilist';

	var _default = {};

	_default.settings =
		{
		};

	_default.options =
		{
			silent: false,
			ignoreChildren: false
		};

	Tree.prototype.template =
		{
			list: '<ul class="list-group"></ul>',
			item: '<li class="list-group-item"></li>',
			checkbox: '<input class="checkbox"></span>',
			text: '<label class="text"></span>',
			//link: '<a href="#" style="color:inherit;"></a>',
			//badge: '<span class="badge"></span>'
		};

	var MultiList = function (element, options)
	{
		this.$element = $(element);
		this.elementId = element.id;

		this.options = this.mergeOptions($.extend({}, options, this.$element.data()));

		this.styleId = this.elementId + '-style';

		 // Initialization.
        this.originalOptions = this.$select.clone()[0].options;
        this.query = '';
        this.searchTimeout = null;
        this.lastToggledInput = null;

		getParent: $.proxy(this.getParent, this),
        this.options.multiple = this.$select.attr('multiple') === "multiple";
        this.options.onChange = $.proxy(this.options.onChange, this);
        this.options.onSelectAll = $.proxy(this.options.onSelectAll, this);
        this.options.onDeselectAll = $.proxy(this.options.onDeselectAll, this);
        this.options.onDropdownShow = $.proxy(this.options.onDropdownShow, this);
        this.options.onDropdownHide = $.proxy(this.options.onDropdownHide, this);
        this.options.onDropdownShown = $.proxy(this.options.onDropdownShown, this);
        this.options.onDropdownHidden = $.proxy(this.options.onDropdownHidden, this);
        this.options.onInitialized = $.proxy(this.options.onInitialized, this);
        this.options.onFiltering = $.proxy(this.options.onFiltering, this);
	}

	Tree.prototype.addItem = function (item, isChecked)
	{
		if (isChecked)
			checkedItems.push(item);
		else
			unCheckedItems.push(item);

		self.append(createListItem(item, isChecked, valuePath, textPath));
	};

	Tree.prototype.createListItem = function (item, isChecked, valuePath, textPath)
	{
		var listItem = $(_this.template.item)
			.addClass(isChecked ? 'active' : '');

		listItem.append(_this.template.checkbox)
			.addAttr('checked', isChecked ? 'checked' : '');

		listItem.append(_this.template.text);



        	return $(
                '<li class="list-group-item ' + (isChecked ? 'active' : '') + '">' +
                '	<label>' +
                '		<input type="checkbox" ' + (isChecked ? 'checked="checked" ' : '') +
                'value="' + item[valuePath] + '" ' +
                '		/> ' + item[textPath] +
                '	</label>' +
                '</li>')
                .change(function () {
                	var $el = $(this).closest('li').toggleClass('active'),
                        item = $el.data('item');
                	
                	if(!$el.prop('checked')){
                		unCheckedItems.splice(unCheckedItems.indexOf(item), 1);
                        checkedItems.push(item);
                    }else{
                        checkedItems.splice(checkedItems.indexOf(item), 1);
                        unCheckedItems.push(item);
                    }   
            }).data('item', item);
        }

	Tree.prototype.getCheckedItems = function ()
	{
		return checkedItems;
	};

	Tree.prototype.getUncheckedItems = function ()
	{
		return unCheckedItems;
	};

	Tree.prototype.getCheckedCheckboxes = function ()
	{
		return self.find('input:checkbox:checked');
	};

	Tree.prototype.getUnCheckedCheckboxes = function ()
	{
		return self.find('input:checkbox:not(checked)');
	};
        
	var logError = function (message)
	{
		if (window.console)
		{
			window.console.error(message);
		}
	};

	// Prevent against multiple instantiations,
	// handle updates and method calls
	$.fn[pluginName] = function (options, args)
	{
		var result;

		this.each(function ()
		{
			var _this = $.data(this, pluginName);
			if (typeof options === 'string')
			{
				if (!_this)
				{
					logError('Not initialized, can not call method : ' + options);
				}
				else if (!$.isFunction(_this[options]) || options.charAt(0) === '_')
				{
					logError('No such method : ' + options);
				}
				else
				{
					if (!(args instanceof Array))
					{
						args = [ args ];
					}

					result = _this[options].apply(_this, args);
				}
			}
			else if (typeof options === 'boolean')
			{
				result = _this;
			}
			else
			{
				$.data(this, pluginName, new Tree(this, $.extend(true, {}, options)));
			}
		});

		return result || this;
	};

})(jQuery, window, document);



(function ()
{
    $.fn.checkList = function (options) {
        var self = this,
            checkedItems = options.checkedItems ? options.checkedItems.slice() : [], //take a copy
            unCheckedItems = options.unCheckedItems ? options.unCheckedItems.slice() : [], //take a copy
            valuePath = options.valuePath,
            textPath = options.textPath;

        this.addClass('list-group');

        $.each(checkedItems, function (key, item) {
            self.append(createListItem(item, true, valuePath, textPath));
        });

        $.each(unCheckedItems, function (key, item) {
            self.append(createListItem(item, false, valuePath, textPath));
        });

        this.addItem = function (item, isChecked) {
            if (isChecked) {
                checkedItems.push(item);
            } else {
                unCheckedItems.push(item);
            }
            self.append(createListItem(item, isChecked, valuePath, textPath));
        };
        this.getCheckedItems=function(){
            return checkedItems;
        };
        this.getUncheckedItems=function(){
            return unCheckedItems;
        };
        this.getCheckedCheckboxes=function(){
            return self.find('input:checkbox:checked');
        }
        this.getUnCheckedCheckboxes=function(){
            return self.find('input:checkbox:not(checked)');
        }
        return this;
            
        function createListItem(item, isChecked, valuePath, textPath) {
        	return $(
                '<li class="list-group-item ' + (isChecked ? 'active' : '') + '">' +
                '	<label>' +
                '		<input type="checkbox" ' + (isChecked ? 'checked="checked" ' : '') +
                'value="' + item[valuePath] + '" ' +
                '		/> ' + item[textPath] +
                '	</label>' +
                '</li>')
                .change(function () {
                	var $el = $(this).closest('li').toggleClass('active'),
                        item = $el.data('item');
                	
                	if(!$el.prop('checked')){
                		unCheckedItems.splice(unCheckedItems.indexOf(item), 1);
                        checkedItems.push(item);
                    }else{
                        checkedItems.splice(checkedItems.indexOf(item), 1);
                        unCheckedItems.push(item);
                    }   
            }).data('item', item);
        }
    }

    $.fn.checkListWithAddControl = function (options) {
        var ul, input;

        this.html(
            '<ul></ul>' +
            '<div class="input-group">' +
            '	<input type="text" class="form-control" placeholder="Add item">' +
            '	<span class="input-group-btn">' +
            '		<button class="btn btn-default" type="button">' +
            '			<i class="glyphicon glyphicon-plus"></i>' +
            '		</button>' +
            '	</span>' +
            '</div>');

        input = this.find('input');

        ul = this.find('ul').checkList(options);

        input.keypress(function (e) {
            var code = e.keyCode || e.which
            if (code == 13) {
                onAdd();
                e.preventDefault();
                return false;
            }
        });
        this.find('button').click(onAdd);

        function onAdd() {
            var value = input.val();
            if (value && options.onAdd) {
                options.onAdd(value, ul.addItem); //checkList.addItem
            }
            input.val('');
        }
        this.getCheckedItems = ul.getCheckedItems;
        this.getUnCheckedItems = ul.getUncheckedItems;
        this.getCheckedCheckboxes = ul.getCheckedCheckboxes;
        this.getUnCheckedCheckboxes = ul.getUnCheckedCheckboxes;
        return this;
    }
})();
$(function () {
    var form = $('form'),
        control = form.find('.control'),
        pre1 = $($('pre')[0]),
        pre2 = $($('pre')[1]),
        options = {
            checkedItems: [{
                text: 'item 1',
                value: 1
            }, {
                text: 'item 2',
                value: 2
            }, {
                text: 'item 3',
                value: 3
            }, {
                text: 'item 4',
                value: 4
            }, {
                text: 'item 5',
                value: 5
            }],
            unCheckedItems: [{
                text: 'item 6',
                value: 6
            }, {
                text: 'item 7',
                value: 7
            }, {
                text: 'item 8',
                value: 8
            }, {
                text: 'item 9',
                value: 9
            }, {
                text: 'item 10',
                value: 10
            }],
            valuePath: 'value',
            textPath: 'text',
            onAdd: function (value, addItemCb) {
                //do api call and in callback call the addItemCallback
                addItemCb({
                    text: value,
                    value: value
                }, true);
            }
        };
    control.checkListWithAddControl(options);
    form.submit(function(e){
        e.preventDefault();
        
        pre1.text(JSON.stringify(control.getCheckedItems()));
        var items = [];
        $.each(control.getCheckedCheckboxes(),function(key, checkbox){
            items.push($(checkbox).closest('li').data('item'));
        });
        pre2.text(JSON.stringify(items));
    });
});