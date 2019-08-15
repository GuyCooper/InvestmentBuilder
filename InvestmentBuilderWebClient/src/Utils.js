'use strict'

function YesNoPicker($uibModalInstance, title, description, name, ok) {
    var $item = this;
    $item.Title = title;
    $item.Description = description;
    $item.Name = name;
    $item.Ok = ok;
    $item.ok = function () {
        $uibModalInstance.close({
            name: $item.Name
        });
    };

    $item.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
};