Vue.component('test-field', {
    props: ['label', 'name', 'value', 'config'],
    data: function () {
        return {
            fields: []
        };
    },
    mounted: function () {
        var self = this;
        fetch("/data/formcomponent")
            .then((stream) => { return stream.json() })
            .then((response) => {
                console.log(response);
                response.forEach(function(component){
                    self.fields.push(component.data.name);
                });
                console.log(self.fields);
            })
        console.log("Value:");
        console.log(this.value);
    },
    template: 
    `<div class="form-group">
        <label v-bind:for="name" > {{ label }}</label> 
        <input class="form-control" type="text" v-bind:id="name" v-bind:value="value" v-on:input="$emit(\'recorechange\', $event.target.value)" />
        <ul>
        <li v-for="field in value">{{field.name}}</li>
        </ul>
    </div >`
});